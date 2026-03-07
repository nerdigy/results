# Nerdigy.Results

A lightweight Result and Error type for .NET applications. Provides a simple, functional approach to error handling without exceptions.

## Installation

```bash
dotnet add package Nerdigy.Results
```

## Usage

### Basic Result (no value)

```csharp
Result result = Result.Success;
Result failure = Result.Failure(new NotFoundError("User not found"));

// Implicit conversion from Error
Result implicit = new BadRequestError("Invalid input");
```

### Result with a value

```csharp
Result<int> result = Result<int>.Success(42);
Result<int> failure = Result<int>.Failure(new InternalError());

// Implicit conversions
Result<int> fromValue = 42;
Result<int> fromError = new NotFoundError("Not found");
```

### Checking outcomes

```csharp
if (result.IsSuccess)
{
    Console.WriteLine(result.Value);
}

if (result.IsFailure)
{
    Console.WriteLine(result.Error);
}
```

### Match

```csharp
var message = result.Match(
    onSuccess: value => $"Got {value}",
    onFailure: error => $"Failed: {error}"
);
```

### Map

```csharp
Result<string> mapped = result.Map(value => value.ToString());
```

### Bind

```csharp
Result<string> bound = result.Bind(value => Result<string>.Success(value.ToString()));
```

### Tap

```csharp
result.Tap(value => Console.WriteLine($"Side effect: {value}"));
```

### Async variants

All core operations have async counterparts: `MatchAsync`, `MapAsync`, `BindAsync`, and `TapAsync`.

```csharp
var message = await result.MatchAsync(
    onSuccess: async value => await FormatAsync(value),
    onFailure: async error => await LogErrorAsync(error)
);
```

## Error Types

The library includes a set of built-in error types that map to common HTTP status codes:

| Error Type                | HTTP Status |
| ------------------------- | ----------- |
| `BadRequestError`         | 400         |
| `ValidationError`         | 400         |
| `UnauthorizedError`       | 401         |
| `ForbiddenError`          | 403         |
| `NotFoundError`           | 404         |
| `ConflictError`           | 409         |
| `UnprocessableError`      | 422         |
| `TooManyRequestsError`    | 429         |
| `InternalError`           | 500         |
| `ServiceUnavailableError` | 503         |
| `GatewayTimeoutError`     | 504         |

All error types are records that extend the base `Error` record, so you can create custom errors by extending `Error` as well.

## License

MIT
