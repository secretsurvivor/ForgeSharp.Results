using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.CodeAnalysis;

namespace ForgeSharp.Results.AspNetCore.Configuration;

public interface IResultErrorMapper<TError>
{
    ProblemDetails ConvertError(TError error);
}

public interface IResultErrorMapperFactory
{
    bool TryGetMapper<TError>([MaybeNullWhen(false)] out IResultErrorMapper<TError> mapper);
}