# ForgeSharp.Results

**ForgeSharp.Results** is a high-performance, type-safe, and allocation-free result monad for .NET. It provides a robust alternative to exceptions for error handling, enabling explicit, predictable, and maintainable code.

---

## Philosophy

- **Performance by Design:**  
  All result types are implemented as readonly structs, ensuring zero heap allocations and minimal GC pressure. This makes ForgeSharp.Results ideal for high-throughput and low-latency applications.

- **Explicit Error Handling:**  
  The result pattern replaces exceptions for flow control, making error and success states explicit and easy to reason about.

- **Type Safety & Clarity:**  
  APIs are strongly-typed and expressive, enabling compile-time safety and clear intent in your code.

- **Modern C# Best Practices:**  
  Leverages the latest C# features and .NET standards for maximum compatibility and future-proofing.

---

## Why Structs?

- **No Hidden Allocations:**  
  Using structs avoids heap allocations, making results lightweight and fast.
- **Value Semantics:**  
  Results are passed by value, ensuring predictable and thread-safe behavior.
- **Immutability:**  
  All result types are readonly, preventing accidental mutation.

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

---

## Quick Start

### Basic Usage

```csharp
Result result = DoSomething();
if (!result.IsSuccess)
    Console.WriteLine(result.Message);

Result<int> valueResult = GetValue();
int value = valueResult.IsSuccess ? valueResult.Value : -1;
```

### Capturing Exceptions

```csharp
Result result = Result.Capture(() => MightThrow());
Result<int> valueResult = Result.Capture(() => MightThrowAndReturnInt());
```

### Async Support

```csharp
Result result = await Result.CaptureAsync(async () => await MightThrowAsync());
Result<int> valueResult = await Result.CaptureAsync(async () => await MightThrowAndReturnIntAsync());
```

---

## API Overview

- **Result / Result<T>:**  
  Core result types representing success, validation faults, and exceptions.
- **IResult / IResult<T>:**  
  Interfaces for result types.
- **Extension Methods:**  
  For mapping, chaining, error handling, and async workflows.
- **Forwarding and Conversion:**  
  Easily convert between generic and non-generic results.

---

## Project Structure

- `Result.cs` – Core result types and logic.
- `Monad/` – Extension methods for chaining, mapping, flattening, and more.

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

**ForgeSharp.Results** – Fast, type-safe, and allocation-free result handling for .NET.
