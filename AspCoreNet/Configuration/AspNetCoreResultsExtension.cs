using ForgeSharp.Results.AspNetCore.Core;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Threading.Channels;

namespace ForgeSharp.Results.AspNetCore.Configuration;

public interface IResultsAspNetCoreConfig
{
    IResultsAspNetCoreConfig RegisterMapper<TMapper, TError>() where TMapper : IResultErrorMapper<TError>;
    IResultsAspNetCoreConfig RegisterMapperFromAssembly<TAssembly>();
    IResultsAspNetCoreConfig RegisterPersister<TPersister>() where TPersister : IStatePersister;
    IResultsAspNetCoreConfig RegisterPersisterFromAssembly<TAssembly>();

    IResultsAspNetCoreConfig LongRunningRequestCapacity(int amount);
}

public static class AspNetCoreResultsExtension
{
    static IEnumerable<(Type implementation, Type error)> FindErrorMappers(Assembly assembly)
    {
        foreach (var type in assembly.GetTypes())
        {
            if (type.IsAbstract || type.IsInterface)
            {
                continue;
            }

            foreach (var i in type.GetInterfaces())
            {
                if (i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IResultErrorMapper<>))
                {
                    yield return (type, i.GetGenericArguments()[0]);
                }
            }
        }
    }

    static IEnumerable<Type> FindPersisters(Assembly assembly)
    {
        foreach (var type in assembly.GetTypes())
        {
            if (type.GetInterfaces().Any(i => i == typeof(IStatePersister)))
            {
                if (type.IsAbstract || type.IsInterface)
                {
                    continue;
                }

                if (typeof(IStatePersister).IsAssignableFrom(type))
                {
                    yield return type;
                }
            }
        }
    }

    public static IServiceCollection AddResults(this IServiceCollection services, Action<IResultsAspNetCoreConfig> config)
    {
        var configImpl = new Config();

        config(configImpl);

        if (configImpl._requestThrottleAmount is null)
        {
            services.AddSingleton(_ => Channel.CreateUnbounded<LongRunningRequest>());
        }
        else
        {
            var channelOption = new BoundedChannelOptions(configImpl._requestThrottleAmount.Value) { FullMode = BoundedChannelFullMode.Wait };
            services.AddSingleton(_ => Channel.CreateBounded<LongRunningRequest>(channelOption));
        }

        services.AddSingleton(_ => Channel.CreateUnbounded<JobProgressUpdate>());
        services.AddSingleton<LongRunningUtility>();
        services.AddSingleton<LongRunningService>();
        services.AddSingleton<IJobRegistry>(sp => sp.GetRequiredService<LongRunningService>());
        services.AddHostedService(sp => sp.GetRequiredService<LongRunningService>());

        foreach (var (implementation, _) in configImpl._mappers)
        {
            services.AddSingleton(implementation);
        }

        services.AddSingleton(x => {
            var factory = new MapperFactory();

            foreach (var (implementation, error) in configImpl._mappers)
            {
                factory.Add(error, () => x.GetRequiredService(implementation));
            }

            return factory;
        });

        foreach (var persister in configImpl._persisters)
        {
            services.AddSingleton(typeof(IStatePersister), persister);
        }

        return services;
    }

    public static IEndpointRouteBuilder AddResultEndpoints(this IEndpointRouteBuilder endpointRouteBuilder)
    {
        var group = endpointRouteBuilder.MapGroup("/__longrunning");
    }

    class MapperFactory : Dictionary<Type, Func<object>>, IResultErrorMapperFactory
    {
        public bool TryGetMapper<TError>([MaybeNullWhen(false)] out IResultErrorMapper<TError> mapper)
        {
            if (TryGetValue(typeof(TError), out var mapperType))
            {
                mapper = (IResultErrorMapper<TError>) mapperType();
                return true;
            }

            mapper = default;
            return false;
        }
    }

    class Config : IResultsAspNetCoreConfig
    {
        public readonly Dictionary<Type, Type> _mappers = [];
        public readonly List<Type> _persisters = [];
        public int? _requestThrottleAmount = null;

        public IResultsAspNetCoreConfig RegisterMapper<TMapper, TError>() where TMapper : IResultErrorMapper<TError>
        {
            if (!_mappers.TryAdd(typeof(TError), typeof(TMapper)))
            {
                throw new ArgumentException($"A mapper for error type '{typeof(TError).Name}' already exists.");
            }

            return this;
        }

        public IResultsAspNetCoreConfig RegisterMapperFromAssembly<TAssembly>()
        {
            foreach (var (implementation, error) in FindErrorMappers(typeof(TAssembly).Assembly))
            {
                if (!_mappers.TryAdd(error, implementation))
                {
                    throw new ArgumentException($"A mapper for error type '{error.Name}' already exists.");
                }
            }

            return this;
        }

        public IResultsAspNetCoreConfig RegisterPersister<TPersister>() where TPersister : IStatePersister
        {
            _persisters.Add(typeof(TPersister));
            return this;
        }

        public IResultsAspNetCoreConfig RegisterPersisterFromAssembly<TAssembly>()
        {
            _persisters.AddRange([.. FindPersisters(typeof(TAssembly).Assembly)]);
            return this;
        }

        public IResultsAspNetCoreConfig LongRunningRequestCapacity(int amount)
        {
            _requestThrottleAmount = amount;
            return this;
        }
    }
}
