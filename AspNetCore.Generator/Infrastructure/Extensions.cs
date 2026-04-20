using Microsoft.CodeAnalysis;
using System;
using System.Security.Cryptography;
using System.Text;

namespace ForgeSharp.Results.AspNetCore.Generator.Infrastructure;

internal static class Extensions
{
    private readonly static SymbolDisplayFormat _uniqueFormat = new SymbolDisplayFormat(
        globalNamespaceStyle: SymbolDisplayGlobalNamespaceStyle.Included,
        typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces,
        memberOptions: SymbolDisplayMemberOptions.IncludeParameters |
                       SymbolDisplayMemberOptions.IncludeContainingType |
                       SymbolDisplayMemberOptions.IncludeModifiers |
                       SymbolDisplayMemberOptions.IncludeExplicitInterface,
        parameterOptions: SymbolDisplayParameterOptions.IncludeType |
                          SymbolDisplayParameterOptions.IncludeName,
        genericsOptions: SymbolDisplayGenericsOptions.IncludeTypeParameters
    );

    public static string ToUniqueName(this ISymbol symbol)
    {
        string display = symbol.ToDisplayString(_uniqueFormat);
        using var sha = SHA256.Create();
        ReadOnlySpan<byte> bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(display));
        string hash = Convert.ToBase64String(bytes.Slice(0, 8).ToArray());

        return hash;
    }
}
