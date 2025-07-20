# ForgeSharp.Results

**ForgeSharp.Results** is a high-performance, type-safe, and allocation-free result monad for .NET. It provides a robust alternative to exceptions for error handling, enabling explicit, predictable, and maintainable code.

---

## Features

- **Performance by Design:**  
  All result types are implemented as readonly structs, ensuring zero heap allocations and minimal GC pressure. This makes ForgeSharp.Results ideal for high-throughput and low-latency applications.

- **Explicit Error Handling:**  
  The result pattern replaces exceptions for flow control, making error and success states explicit and easy to reason about.

- **Type Safety & Clarity:**  
  APIs are strongly-typed and expressive, enabling compile-time safety and clear intent in your code.

- **Modern C# Best Practices:**  
  Leverages the latest C# features and .NET standards for maximum compatibility and future-proofing.

- **Composable Pipelines:**  
  Build synchronous and asynchronous pipelines for result-based workflows, with full LINQ support for expressive, query-based composition.

---

## Pipelines & LINQ Integration

ForgeSharp.Results introduces **Pipelines** for composing complex workflows in a functional, type-safe, and allocation-free manner. Pipelines can be synchronous (`IPipeline<T>`) or asynchronous (`IAsyncPipeline<T>`), and can be created from delegates or composed from other pipelines.

- **LINQ Support:** Pipelines can be composed using LINQ query expressions, enabling powerful, readable, and maintainable workflows. LINQ methods like `select`, `where`, and `from` are supported for both sync and async pipelines.
- **Combining Results:** Pipelines are ideal for combining multiple result-producing operations, propagating errors automatically, and building robust data flows.

_Example:
```csharp
var pipeline =
    from user in GetUserPipeline()
    where user.IsActive
    from profile in GetProfilePipeline(user.Id)
    select (user, profile);

var result = pipeline.Execute();
if (result.IsSuccess)
    Console.WriteLine($"User: {result.Value.user}, Profile: {result.Value.profile}");
```
---

## Extension Methods

ForgeSharp.Results provides a rich set of extension methods to make working with results functional, expressive, and safe in both synchronous and asynchronous scenarios.

### Chaining and Functional Composition

- **Then / ThenAsync**  
  Chain operations that return results, propagating failures automatically.  
  _Example:_  
  `result.Then(x => DoSomething(x))`  
  `await result.ThenAsync(x => DoSomethingAsync(x))`

### Value Extraction and Conversion

- **GetOrDefault / GetOrDefaultAsync**  
  Get the value if successful, or a default value if not.  
  _Example:_  
  `result.GetOrDefault(-1)`  
  `await resultTask.GetOrDefaultAsync(-1)`

- **GetOrThrow / GetOrThrowAsync**  
  Get the value if successful, or throw an exception if not.  
  _Example:_  
  `result.GetOrThrow()`  
  `await resultTask.GetOrThrowAsync()`

- **AsResult / AsResultAsync**  
  Convert a `Result<T>` to a non-generic `Result`.  
  _Example:_  
  `result.AsResult()`  
  `await resultTask.AsResultAsync()`

### Null Safety

- **EnsureNotNull / EnsureNotNullAsync**  
  Ensure the value of a successful result is not null, otherwise return a failed result.  
  _Example:_  
  `result.EnsureNotNull()`  
  `await resultTask.EnsureNotNullAsync()`

### Tapping (Side Effects)

- **Tap / TapAsync**  
  Execute an action if the result is successful, without altering the result.  
  _Example:_  
  `result.Tap(() => Log("Success"))`  
  `await result.TapAsync(async () => await LogAsync())`

### Flattening

- **Flatten / FlattenAsync**  
  Flatten nested results or a sequence of results into a single result, propagating the first failure.  
  _Example:_  
  `result.Flatten()`  
  `await resultTask.FlattenAsync()`

### Mapping

- **Map / MapAsync**  
  Map the values of a successful result sequence to a new type.  
  _Example:_  
  `result.Map(x => x.ToString())`  
  `await resultTask.MapAsync(x => x.ToString())`

### Resolving Collections

- **ResolveAsync**  
  Asynchronously resolve a collection of result tasks into a collection or async enumerable of results.  
  _Example:_  
  `await tasks.ResolveAsync()`

### Dictionary and Collection Helpers

- **TryGetValueResult**  
  Attempts to get a value from a dictionary as a `Result<T>`.  
  _Example:_  
  `var valueResult = dict.TryGetValueResult(key);`

- **FirstOrResult**  
  Returns the first element in a sequence that matches a predicate as a `Result<T>`.  
  _Example:_  
  `var firstResult = list.FirstOrResult(x => x.IsActive, "No active item found.");`

### Miscellaneous

- **Restore / RestoreAsync**  
  Restore a failed result using a provided function, useful for fallback or recovery logic.  
  _Example:_  
  `result.Restore(fallbackFunc)`  
  `await resultTask.RestoreAsync(fallbackFunc)`

- **IsSuccessAsync**  
  Determine whether an awaited result is successful.  
  _Example:_  
  `await resultTask.IsSuccessAsync()`

---

## Use Cases

- **Error Handling Without Exceptions:**  
  Replace exceptions for flow control with explicit, type-safe results.

```csharp
Result result = DoWork();
if (!result.IsSuccess)
    Console.WriteLine(result.Message);
```

- **Functional Pipelines:**  
  Chain operations and propagate errors fluently.

```csharp
var result = GetUser()
      .Then(user => Validate(user))
      .Then(validUser => Save(validUser));
```

- **Async Pipelines:**  
  Use async/await with result types for modern, non-blocking code.

```csharp
var result = await GetUserAsync()
      .ThenAsync(user => ValidateAsync(user));
```

- **LINQ Pipelines:**  
  Use LINQ query expressions to compose pipelines and combine results.
```csharp
var pipeline =
    from user in GetUserPipeline()
    where user.IsActive
    from profile in GetProfilePipeline(user.Id)
    select (user, profile);

var result = pipeline.Execute();
```
---

## Quick Start

### Basic Usage

Results can be used to represent success or failure in operations, with optional messages and exceptions.

```csharp
Result DoSomething()
{
    // Perform some operation
    return Result.Success();
}

Result result = DoSomething();
if (!result.IsSuccess)
    Console.WriteLine(result.Message);
```

Results can also carry values, allowing you to return data alongside success or failure states.

```csharp
Result<int> GetValue()
{
    // Perform some operation that returns an int
    return Result.Success(42);
}

Result<int> valueResult = GetValue();
int value = valueResult.IsSuccess ? valueResult.Value : -1;
```

### Capturing Exceptions

```csharp
Result result = Result.Capture(() => MightThrow());
Result<int> valueResult = Result.Capture(() => MightThrowAndReturnInt());

Result result = await Result.CaptureAsync(async () => await MightThrowAsync());
Result<int> valueResult = await Result.CaptureAsync(async () => await MightThrowAndReturnIntAsync());
```

---

## API Overview

- **Result / Result<T>:**  
  Core result types representing success, validation faults, and exceptions.
- **IResult / IResult<T>:**  
  Interfaces for result types.
- **Pipelines:**  
  Synchronous and asynchronous pipelines for composing result-based workflows, with LINQ support.
- **Functional Extension Methods:**  
  For mapping, chaining, error handling, and async workflows.
- **Dictionary and Collection Helpers:**  
  Methods like `TryGetValueResult` and `FirstOrResult` for safer collection access.
- **Forwarding and Conversion:**  
  Easily convert between generic and non-generic results.

---

## Project Structure

- `Result.cs` - Core result types and logic.
- `Pipeline.cs` - Pipeline interfaces and implementations.
- `Monad/` - Extension methods for chaining, mapping, flattening, and more.

---

## Requirements

- .NET Standard 2.0, .NET Standard 2.1, or .NET 8.0+

---

## Contributing

Contributions are welcome! Please open issues or submit pull requests for bug fixes, improvements, or new features.

---

## License

[MIT License](LICENSE)

---

## Acknowledgements

- Inspired by functional programming and modern .NET design.
- Built for developers who care about performance, safety, and maintainability.

---

**ForgeSharp.Results** - Fast, type-safe, and allocation-free result handling for .NET.
