# AspNetCore Integration for ForgeSharp.Results

This is a simple integration for ForgeSharp.Results at the moment with more to come.
It provides the ResultEndpoint type to automatically serialise the Result type. Use in
combination with Result<,> to provide rich error mapping.

"Automatically" in this case requires result error mappers to map the error type into
ProblemDetails using the `IResultErrorMapper<>` interface and injecting it with
dependancy injection.

```csharp
builder.Services.AddResults(config => {
    // You can register all mappers across an assembly
    config.RegisterMapperFromAssembly<Program>();

    // or inject individually manually
    config.RegisterMapper<DomainErrorMapper, DomainError>();
    // Using both will cause errors
});

// Always inject the Problem Details Service
builder.Services.AddProblemDetails();
```

Once you have the mapper registered then you are free to create endpoints that use
`ResultEndpoint`. Use `ResultEndpoint` as the return type and return a ForgeSharp
`Result` type and the serialisation of the response will be automatically handled
by the type.

```csharp
[HttpPost]
public async Task<ResultEndpoint<UserSaveResponse, DomainError>> CreateUser(UserSaveRequest request, [FromServices] IUserServices userServices)
{
    // In this case SaveUserAsync returns Result<UserSaveResponse, DomainError>
    return await userServices.SaveUserAsync(request);
}
```

Problem details _RFC 9457_ is a web standard with lots of material on the best practise
to use it well. This is not a good example of how to fill the fields but just one to
present the IResultErrorMapper

```csharp
internal sealed class DomainErrorMapper : IResultErrorMapper<DomainError>
{
    public ProblemDetails ConvertError(DomainError error)
    {
        if (error is DomainError.ValidationError validationError)
        {
            return new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Bad Request",
                Detail = validationError.Message,
            };
        }

        if (error is DomainError.ExceptionError exceptionError)
        {
            return new ProblemDetails
            {
                Status = StatusCodes.Status500InternalServerError,
                Title = "Unexpected error occurred",
            };
        }

        throw new InvalidDataException($"Invalid error type: {error.GetType().Name}");
    }
}
```