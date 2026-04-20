#nullable enable

using ForgeSharp.Results.AspNetCore.Generator.Infrastructure;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace ForgeSharp.Results.AspNetCore.Generator;

[Generator]
public class PipelineEndpointGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var controllerBase = context.CompilationProvider.Select(static (c, _) => c.GetTypeByMetadataName("Microsoft.AspNetCore.Mvc.ControllerBase"));
        var compilationInputs = context.CompilationProvider.Select(static (c, _) => new CompilationInputs(c));

        var declarations = context.SyntaxProvider.CreateSyntaxProvider(
            predicate: static (n, ct) => {
                if (n is not ClassDeclarationSyntax
                    {
                        BaseList: not null,
                        Members: var members,
                        Modifiers: var modifiers
                    })
                {
                    return false;
                }

                foreach (var modifier in modifiers)
                {
                    if (modifier.IsKind(SyntaxKind.AbstractKeyword) || modifier.IsKind(SyntaxKind.PrivateKeyword))
                    {
                        return false;
                    }
                }

                foreach (var member in members)
                {
                    if (member is MethodDeclarationSyntax { AttributeLists.Count: > 0 })
                    {
                        return true;
                    }
                }

                return false;
            },
            transform: static (n, ct) => {
                var syntax = (ClassDeclarationSyntax) n.Node;
                var symbol = n.SemanticModel.GetDeclaredSymbol(syntax);

                if (symbol is not INamedTypeSymbol namedSymbol)
                {
                    return default;
                }

                return (success: true, namedSymbol);
            })
            .Combine(controllerBase)
            .Select(static (symbols, _) => {
                var (candidate, controllerBase) = symbols;

                if (!candidate.success || controllerBase is null)
                {
                    return default;
                }

                var (_, namedSymbol) = candidate;

                for (var type = namedSymbol.BaseType; type is not null; type = type.BaseType)
                {
                    if (SymbolEqualityComparer.Default.Equals(type, controllerBase))
                    {
                        return (success: true, namedSymbol);
                    }
                }

                return default;
            })
            .Combine(compilationInputs.Select((x, _) => (x.PipelineAttributes, x.ResultEndpointTypes)))
            .Select(static (symbols, _) => {
                var (candidate, (attributes, resultTypes)) = symbols;

                if (!candidate.success || !attributes.IsValid || !resultTypes.IsValid)
                {
                    return default;
                }

                var (_, symbol) = candidate;
                var methods = GetMethods(symbol, attributes, resultTypes).ToImmutableArray();

                static IEnumerable<IMethodSymbol> GetMethods(INamedTypeSymbol symbol, PipelineAttributes attributes, ResultEndpointTypes resultTypes)
                {
                    foreach (var member in symbol.GetMembers())
                    {
                        if (member is not IMethodSymbol method || !resultTypes.IsResultEndpoint(method.ReturnType))
                        {
                            continue;
                        }
                        else if (method.IsStatic || method.DeclaredAccessibility is not Accessibility.Public)
                        {
                            continue;
                        }

                        foreach (var attr in method.GetAttributes())
                        {
                            if (attr.AttributeClass is not INamedTypeSymbol attributeClass)
                            {
                                continue;
                            }

                            if (attributes.IsTargetAttribute(attributeClass))
                            {
                                yield return method;
                            }
                        }
                    }
                }

                if (methods.Length > 0)
                {
                    return (success: true, symbol, methods);
                }

                return default;
            })
            .Where(static x => x.success);

        context.RegisterSourceOutput(declarations.Combine(compilationInputs), static (context, input) => {
            var ((_, symbol, methods), inputs) = input;
            Generate(context, symbol, methods, inputs);
        });
    }

    private static void Generate(SourceProductionContext context, INamedTypeSymbol symbol, ImmutableArray<IMethodSymbol> methods, CompilationInputs inputs)
    {
        var (attribute, taskTypes, resultType) = inputs;
        var classSyntax = SyntaxFactory.ClassDeclaration($"__Generated_{symbol.Name}_PipelineInterceptor")
            .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword), SyntaxFactory.Token(SyntaxKind.StaticKeyword))
            .AddMembers([.. GenerateMembers(symbol, methods, inputs).Select(x => x.WithTrailingTrivia(SyntaxFactory.ElasticCarriageReturnLineFeed))]);

        static IEnumerable<MemberDeclarationSyntax> GenerateMembers(INamedTypeSymbol controllerSymbol, ImmutableArray<IMethodSymbol> methods, CompilationInputs inputs)
        {
            foreach (var method in methods)
            {
                inputs.AnalyseMethod(method, out bool isAsync, out var typeArguments);

                var resultEndpointType = SyntaxFactory.ParseTypeName(method.ReturnType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat));
                var pipelineType = SyntaxFactory.QualifiedName(
                    SyntaxFactory.QualifiedName(
                        SyntaxFactory.IdentifierName("ForgeSharp"),
                        SyntaxFactory.IdentifierName("Results")),
                    SyntaxFactory.GenericName(SyntaxFactory.Identifier(isAsync ? "IAsyncPipeline" : "IPipeline"))
                        .WithTypeArgumentList(SyntaxFactory.TypeArgumentList(
                            SyntaxFactory.SeparatedList(typeArguments.Select(t => SyntaxFactory.ParseTypeName(t.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)))))));

                var resultType = SyntaxFactory.QualifiedName(
                    SyntaxFactory.QualifiedName(
                        SyntaxFactory.IdentifierName("ForgeSharp"),
                        SyntaxFactory.IdentifierName("Results")),
                    SyntaxFactory.GenericName(SyntaxFactory.Identifier("Result"))
                        .WithTypeArgumentList(SyntaxFactory.TypeArgumentList(
                            SyntaxFactory.SeparatedList(typeArguments.Select(t => SyntaxFactory.ParseTypeName(t.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)))))));

                static TypeSyntax WrapInTask(QualifiedNameSyntax typeSyntax)
                {
                    return SyntaxFactory.QualifiedName(
                        SyntaxFactory.QualifiedName(
                            SyntaxFactory.IdentifierName("System.Threading"),
                            SyntaxFactory.IdentifierName("Tasks")),
                        SyntaxFactory.GenericName(SyntaxFactory.Identifier("Task"))
                            .WithTypeArgumentList(SyntaxFactory.TypeArgumentList(
                                SyntaxFactory.SeparatedList<TypeSyntax>([typeSyntax]))));
                }

                var invocation = SyntaxFactory.InvocationExpression(
                        SyntaxFactory.MemberAccessExpression(
                            SyntaxKind.SimpleMemberAccessExpression,
                            SyntaxFactory.IdentifierName("controller"),
                            SyntaxFactory.IdentifierName(method.Name)))
                    .WithArgumentList(SyntaxFactory.ArgumentList(SyntaxFactory.SeparatedList(method.Parameters.Select(x => SyntaxFactory.Argument(SyntaxFactory.IdentifierName(x.Name))))));

                var structSyntax = SyntaxFactory.StructDeclaration($"__Generated_{method.ToUniqueName()}")
                    .AddBaseListTypes(SyntaxFactory.SimpleBaseType(pipelineType))
                    .AddParameterListParameters([
                        SyntaxFactory.Parameter(SyntaxFactory.Identifier("controller"))
                            .WithType(SyntaxFactory.ParseTypeName(controllerSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat))),
                        ..method.Parameters.Select(x => SyntaxFactory.Parameter(SyntaxFactory.Identifier(x.Name))
                            .WithType(SyntaxFactory.ParseTypeName(x.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat))))
                    ])
                    .AddMembers(
                        SyntaxFactory.MethodDeclaration(
                            returnType: isAsync ? WrapInTask(resultType) : resultType,
                            identifier: isAsync ? "ExecuteAsync" : "Execute")
                        .AddModifiers(
                            SyntaxFactory.Token(SyntaxKind.PublicKeyword),
                            isAsync ? SyntaxFactory.Token(SyntaxKind.AsyncKeyword) : default)
                        .AddParameterListParameters(isAsync
                            ? [
                                SyntaxFactory.Parameter(SyntaxFactory.Identifier("cancellationToken"))
                                    .WithType(SyntaxFactory.ParseTypeName("global::System.Threading.CancellationToken"))
                                    .WithDefault(SyntaxFactory.EqualsValueClause(
                                        SyntaxFactory.LiteralExpression(
                                            SyntaxKind.DefaultLiteralExpression,
                                            SyntaxFactory.Token(SyntaxKind.DefaultKeyword))))
                              ]
                            : [])
                        .WithBody(SyntaxFactory.Block(
                            SyntaxFactory.ReturnStatement(
                                isAsync
                                    ? SyntaxFactory.AwaitExpression(invocation)
                                    : invocation))));

                yield return structSyntax;

                // ReplaceMethod — mirrors the original controller method signature
                var replaceMethod = SyntaxFactory.MethodDeclaration(
                        returnType: resultEndpointType,
                        identifier: method.Name)
                    .AddModifiers(
                        SyntaxFactory.Token(SyntaxKind.PublicKeyword),
                        SyntaxFactory.Token(SyntaxKind.StaticKeyword),
                        SyntaxFactory.Token(SyntaxKind.AsyncKeyword))
                    .AddParameterListParameters([
                        SyntaxFactory.Parameter(SyntaxFactory.Identifier("controller"))
                    .WithType(SyntaxFactory.ParseTypeName(controllerSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat))),
                        ..method.Parameters.Select(x =>
                            SyntaxFactory.Parameter(SyntaxFactory.Identifier(x.Name))
                        .WithType(SyntaxFactory.ParseTypeName(x.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat))))])
                    .WithBody(SyntaxFactory.Block(
                        SyntaxFactory.ReturnStatement(
                            SyntaxFactory.AwaitExpression(
                                SyntaxFactory.InvocationExpression(
                                    SyntaxFactory.MemberAccessExpression(
                                        SyntaxKind.SimpleMemberAccessExpression,
                                        // new __Generated_Bang(controller, ...args)
                                        SyntaxFactory.ObjectCreationExpression(
                                                SyntaxFactory.ParseTypeName($"__Generated_{method.ToUniqueName()}"))
                                            .WithArgumentList(SyntaxFactory.ArgumentList(
                                                SyntaxFactory.SeparatedList([
                                                    SyntaxFactory.Argument(SyntaxFactory.IdentifierName("controller")),
                                    ..method.Parameters.Select(x =>
                                        SyntaxFactory.Argument(SyntaxFactory.IdentifierName(x.Name)))
                                                ]))),
                                        SyntaxFactory.IdentifierName(isAsync ? "ExecuteAsync" : "Execute")))
                                .WithArgumentList(SyntaxFactory.ArgumentList(
                                    isAsync
                                        ? SyntaxFactory.SingletonSeparatedList(
                                            SyntaxFactory.Argument(
                                                SyntaxFactory.LiteralExpression(
                                                    SyntaxKind.DefaultLiteralExpression,
                                                    SyntaxFactory.Token(SyntaxKind.DefaultKeyword))))
                                        : SyntaxFactory.SeparatedList<ArgumentSyntax>([])))))));

                yield return replaceMethod;
            }
        }
    }

    class CompilationInputs(Compilation c)
    {
        public PipelineAttributes PipelineAttributes { get; } = new PipelineAttributes(c);
        public TaskTypes TaskTypes { get; } = new TaskTypes(c);
        public ResultEndpointTypes ResultEndpointTypes { get; } = new ResultEndpointTypes(c);

        public void Deconstruct(out PipelineAttributes attribute, out TaskTypes taskTypes, out ResultEndpointTypes resultType)
        {
            attribute = PipelineAttributes;
            taskTypes = TaskTypes;
            resultType = ResultEndpointTypes;
        }

        public void AnalyseMethod(IMethodSymbol methodSymbol, out bool isAsync, out ImmutableArray<ITypeSymbol> typeArguments)
        {
            var returnType = methodSymbol.ReturnType;

            isAsync = TaskTypes.IsTaskType(returnType, out var innerType);

            if (innerType is null)
            {
                typeArguments = default;
                return;
            }

            typeArguments = innerType.TypeArguments;
        }
    }

    class PipelineAttributes(Compilation c)
    {
        public INamedTypeSymbol? BulkheadAttribute { get; } = c.GetTypeByMetadataName(Namespace.Combine("Attributes", "BulkheadAttribute"));
        public INamedTypeSymbol? DebounceAttribute { get; } = c.GetTypeByMetadataName(Namespace.Combine("Attributes", "DebounceAttribute"));
        public INamedTypeSymbol? IdempotentAttribute { get; } = c.GetTypeByMetadataName(Namespace.Combine("Attributes", "IdempotentAttribute"));
        public INamedTypeSymbol? LongRunningAttribute { get; } = c.GetTypeByMetadataName(Namespace.Combine("Attributes", "LongRunningAttribute"));
        public INamedTypeSymbol? RetryAttribute { get; } = c.GetTypeByMetadataName(Namespace.Combine("Attributes", "RetryAttribute"));
        public INamedTypeSymbol? TimeoutAttribute { get; } = c.GetTypeByMetadataName(Namespace.Combine("Attributes", "TimeoutAttribute"));

        public IEnumerable<INamedTypeSymbol?> Attributes
        {
            get
            {
                yield return BulkheadAttribute;
                yield return DebounceAttribute;
                yield return IdempotentAttribute;
                yield return LongRunningAttribute;
                yield return RetryAttribute;
                yield return TimeoutAttribute;
            }
        }

        public bool IsValid
        {
            get
            {
                foreach (var attribute in Attributes)
                {
                    if (attribute is null)
                    {
                        return false;
                    }
                }

                return true;
            }
        }

        public bool IsTargetAttribute(INamedTypeSymbol symbol)
        {
            foreach (var attribute in Attributes)
            {
                if (SymbolEqualityComparer.Default.Equals(symbol, attribute))
                {
                    return true;
                }
            }

            return false;
        }
    }

    class TaskTypes(Compilation c)
    {
        public INamedTypeSymbol? Task { get; } = c.GetTypeByMetadataName("System.Threading.Tasks.Task");
        public INamedTypeSymbol? TaskT { get; } = c.GetTypeByMetadataName("System.Threading.Tasks.Task`1");
        public INamedTypeSymbol? ValueTask { get; } = c.GetTypeByMetadataName("System.Threading.Tasks.ValueTask");
        public INamedTypeSymbol? ValueTaskT { get; } = c.GetTypeByMetadataName("System.Threading.Tasks.ValueTask`1");

        public bool IsTaskType(ITypeSymbol typeSymbol, out INamedTypeSymbol? innerType)
        {
            if (SymbolEqualityComparer.Default.Equals(typeSymbol, Task))
            {
                innerType = default;
                return true;
            }

            if (typeSymbol is INamedTypeSymbol namedSymbol && SymbolEqualityComparer.Default.Equals(namedSymbol.OriginalDefinition, TaskT))
            {
                innerType = namedSymbol.TypeArguments[0] as INamedTypeSymbol;
                return true;
            }

            innerType = default;
            return false;
        }
    }

    class ResultEndpointTypes(Compilation c)
    {
        public INamedTypeSymbol? ResultEndpoint { get; } = c.GetTypeByMetadataName(Namespace.Combine("ResultEndpoint"));
        public INamedTypeSymbol? ResultEndpointT { get; } = c.GetTypeByMetadataName(Namespace.Combine("ResultEndpoint`1"));
        public INamedTypeSymbol? ResultEndpointTError { get; } = c.GetTypeByMetadataName(Namespace.Combine("ResultEndpoint`2"));

        public bool IsValid => ResultEndpoint is not null && ResultEndpointT is not null && ResultEndpointTError is not null;

        public bool IsResultEndpoint(ITypeSymbol typeSymbol)
        {
            if (SymbolEqualityComparer.Default.Equals(typeSymbol, ResultEndpoint))
            {
                return true;
            }

            if (typeSymbol is INamedTypeSymbol namedSymbol
                && (SymbolEqualityComparer.Default.Equals(namedSymbol.OriginalDefinition, ResultEndpointT)
                || SymbolEqualityComparer.Default.Equals(namedSymbol.OriginalDefinition, ResultEndpointTError)))
            {
                return true;
            }

            return false;
        }
    }
}
