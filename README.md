# ForgeSharp.Results

A monadic `Result` type for .NET that replaces exceptions as flow control with explicit, composable success/failure values. All core types are `readonly struct` — zero heap allocations, zero GC pressure.

| Target | Supported |
|---|---|
| .NET 10 | Yes |
| .NET 8 | Yes |
| .NET Standard 2.1 | Yes |
| .NET Standard 2.0 | Yes |

## Install

```
dotnet add package ForgeSharp.Results
```

## Get started

Return a `Result` from any operation. Check `IsSuccess` to branch on the outcome — no try/catch required.

```csharp
Result<User> GetUser(int id)
{
    var user = db.Find(id);
    return user is not null
        ? Result.Ok(user)
        : Result.Fail<User>("User not found.");
}

var result = GetUser(42);
if (result.IsSuccess)
    Console.WriteLine(result.Value.Name);
else
    Console.WriteLine(result.Message);
```

Chain operations with `Map` and `Bind`. Failures propagate automatically — no manual error checking between steps.

```csharp
var result = GetUser(42)
    .Bind(user => Validate(user))
    .Map(user => user.Profile);
```

Wrap code that throws into a `Result` with `Capture`:

```csharp
Result result = Result.Capture(() => RiskyOperation());
Result<int> parsed = Result.Capture(() => int.Parse(input));

// Async
Result<Data> data = await Result.CaptureAsync(() => httpClient.GetFromJsonAsync<Data>(url));
```

## Core types

| Type | Description |
|---|---|
| `Result` | Non-generic success/failure. |
| `Result<T>` | Carries a `T` value on success. |
| `Result<TValue, TError>` | Discriminated result with a custom error type. |
| `Options<T>` | Optional value wrapper (`Some` / `None`). |
| `EnumerableResult` / `EnumerableResult<T>` | Aggregated batch results with counts, ratios, and filtering. |

Results distinguish three states: **success**, **validation fault** (with a message), and **exception** (with a captured `Exception`). The `IsSuccess`, `IsValidationFault`, and `IsException` properties let you branch accordingly.

## Pipelines

Pipelines compose result-producing operations into discrete, reusable units. They come in sync (`IPipeline<T>`) and async (`IAsyncPipeline<T>`) variants and support LINQ query syntax.

```csharp
var pipeline =
    from user in GetUserPipeline()
    where user.IsActive
    from profile in GetProfilePipeline(user.Id)
    select (user, profile);

Result<(User user, Profile profile)> result = pipeline.Execute();
```

Pipelines also support **retry** and **timeout** policies:

```csharp
var resilient = Pipeline.Create(() => CallExternalService())
    .Retry(3, TimeSpan.FromSeconds(1))
    .Timeout(TimeSpan.FromSeconds(10));

Result result = resilient.Execute();
```

Use `Repeat` to execute a pipeline multiple times and collect the results:

```csharp
EnumerableResult batch = pipeline.Repeat(100).ToEnumerableResult();
Console.WriteLine($"{batch.SuccessPercentage}% succeeded");
```

## Extension methods

Every extension ships with an async counterpart (e.g. `Map` / `MapAsync`).

### Chaining

| Method | Purpose |
|---|---|
| `Map` | Transform the value if successful. |
| `Bind` | FlatMap — chain an operation that itself returns a `Result`. |
| `Select` | Project the value to a new type. |
| `Flatten` | Unwrap a nested `Result<Result<T>>`. |

```csharp
var name = GetUser(42)
    .Bind(u => LoadSettings(u.Id))
    .Map(s => s.DisplayName)
    .GetOrDefault("Unknown");
```

### Side effects

| Method | Purpose |
|---|---|
| `Tap` | Execute an action on success without changing the result. |
| `TapError` | Execute an action on failure without changing the result — great for logging. |
| `Restore` | Recover from a failure with a fallback function. |

```csharp
var result = GetUser(42)
    .Tap(user => Log.Info($"Found {user.Name}"))
    .TapError(err => Log.Warn($"Lookup failed: {err.Message}"));
```

### Error transformation

| Method | Purpose |
|---|---|
| `MapError` | Transform the error of a `Result<TValue, TError>` while preserving the value. |

```csharp
Result<User, ApiError> apiResult = CallApi();
Result<User, string> friendly = apiResult.MapError(e => e.UserMessage);
```

### Value extraction

| Method | Purpose |
|---|---|
| `GetOrDefault` | Returns the value, or a fallback if failed. |
| `GetOrThrow` | Returns the value, or re-throws the captured exception. |
| `EnsureNotNull` | Converts a `Result<T?>` to `Result<T>`, failing on null. |
| `AsResult` | Drops the value, converting `Result<T>` to `Result`. |
| `TryGetValue` | Try-pattern extraction (`bool` + `out T`). |
| `ToOption` | Converts a result to `Options<T>` — `Some` on success, `None` on failure. |

### Collections

| Method | Purpose |
|---|---|
| `TryGetValueResult` | Dictionary lookup that returns `Result<T>` instead of `bool`. |
| `FirstOrResult` | First matching element, or a failure with a message. |
| `ResolveAsync` | Awaits a collection of `Task<Result<T>>` into aggregated results. |
| `FilterValues` / `FilterExceptions` | Extract successes or failures from an `EnumerableResult<T>`. |
| `AggregateValidation` | Combines all validation messages into a single result. |
| `ExtractValuesOrDefault` | All values from a batch, substituting a default for failures. |

## Options interop

Convert freely between `Options<T>` and `Result<T>`:

```csharp
// Result → Option
Options<User> maybeUser = GetUser(42).ToOption();

// Option → Result
Result<User> result = maybeUser.ToResult("No user found.");

// With a discriminated error type
Result<User, MyError> typed = maybeUser.ToResult(new MyError("missing"));
```

## I/O integration

File, directory, and HTTP operations are wrapped as `Result`-returning extension methods so you never need a try/catch around I/O.

### File & directory

```csharp
Result<FileStream> stream = new FileInfo("data.bin").OpenReadAsResult();
Result deleted = File.DeleteAsResult("temp.log");

Result<DirectoryInfo> dir = Directory.CreateAsResult(@"C:\App\Logs");
Result removed = new DirectoryInfo(@"C:\App\Old").DeleteAsResult();
```

### HttpClient

Every HTTP verb has an `AsResult` variant:

```csharp
Result<HttpResponseMessage> response = await httpClient.GetAsResultAsync("https://api.example.com/items");

Result<Item[]> items = await httpClient
    .GetAsResultAsync("https://api.example.com/items")
    .ThenEnsureSuccessStatusCodeAsync(
        success: r => r.Content.ReadFromJsonAsync<Item[]>(),
        failure: r => $"API returned {r.StatusCode}");
```

## API reference

| Area | Key types / methods |
|---|---|
| **Core** | `Result`, `Result<T>`, `Result<TValue, TError>`, `Options<T>` |
| **Pipelines** | `IPipeline<T>`, `IAsyncPipeline<T>`, `Pipeline.Create(...)` |
| **Monad** | `Map`, `MapError`, `Bind`, `Select`, `Flatten`, `Tap`, `TapError`, `Restore`, `Retry`, `Timeout`, `Repeat` |
| **Extraction** | `GetOrDefault`, `GetOrThrow`, `EnsureNotNull`, `AsResult`, `TryGetValue`, `ToOption`, `ToResult` |
| **Collections** | `EnumerableResult<T>`, `TryGetValueResult`, `FirstOrResult`, `ResolveAsync`, `AggregateValidation` |
| **Integration** | `OpenReadAsResult`, `DeleteAsResult`, `CreateAsResult`, `SendAsResultAsync`, `ThenEnsureSuccessStatusCode` |
| **LINQ** | `from`/`where`/`select` query syntax on pipelines |

## Contributing

Contributions are welcome. Open an issue or submit a pull request on [GitHub](https://github.com/secretsurvivor/ForgeSharp.Results).

## License

[MIT](LICENSE)
