namespace ForgeSharp.Results.AspNetCore.Generator.Infrastructure;

internal static class Namespace
{
    private const string _libraryNamespace = "ForgeSharp.Results.AspNetCore";

    public static string Combine(params string[] arguments) => string.Join(".", [_libraryNamespace, arguments]);
}
