using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace ForgeSharp.Results.AspNetCore.Configuration;

public interface IResultsAspNetCoreConfig
{
    IResultsAspNetCoreConfig RegisterMapper<TMapper, TError>() where TMapper : IResultErrorMapper<TError>;
    IResultsAspNetCoreConfig RegisterMapperFromAssembly<TAssembly>();
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

    public static IServiceCollection AddResults(this IServiceCollection services, Action<IResultsAspNetCoreConfig> config)
    {
        var configImpl = new Config();

        config(configImpl);

        foreach (var (_, implementation) in configImpl._mappers)
        {
            services.AddSingleton(implementation);
        }

        services.AddSingleton<IResultErrorMapperFactory>(x => {
            var factory = new MapperFactory();

            foreach (var (error, implementation) in configImpl._mappers)
            {
                factory.Add(error, () => x.GetRequiredService(implementation));
            }

            return factory;
        });

        return services;
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
    }
}
