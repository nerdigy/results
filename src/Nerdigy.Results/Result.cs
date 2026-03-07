namespace Nerdigy.Results;

/// <summary>
/// Represents the outcome of an operation that does not return a value.
/// </summary>
public readonly struct Result
{
    /// <summary>
    /// The error associated with a failed result.
    /// </summary>
    private readonly Error? _error;

    /// <summary>
    /// Gets a value indicating whether the result represents a successful outcome.
    /// </summary>
    public bool IsSuccess => _error is null;

    /// <summary>
    /// Gets a value indicating whether the result represents a failed outcome.
    /// </summary>
    public bool IsFailure => _error is not null;

    /// <summary>
    /// Gets the error associated with a failed result.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when accessed on a successful result.</exception>
    public Error Error => IsFailure ? _error! : throw new InvalidOperationException("Cannot access Error when Result is successful.");

    /// <summary>
    /// Initializes a new instance of the <see cref="Result"/> struct with the specified error.
    /// </summary>
    /// <param name="error">The error representing the failure.</param>
    private Result(Error error)
    {
        _error = error;
    }

    /// <summary>
    /// Gets a successful result.
    /// </summary>
    public static Result Success => new();

    /// <summary>
    /// Creates a failed result with the specified error.
    /// </summary>
    /// <param name="error">The error representing the failure.</param>
    /// <returns>A failed <see cref="Result"/>.</returns>
    public static Result Failure(Error error) => new(error);

    /// <summary>
    /// Implicitly converts an <see cref="Error"/> to a failed <see cref="Result"/>.
    /// </summary>
    /// <param name="error">The error to convert.</param>
    public static implicit operator Result(Error error) => Failure(error);
}

/// <summary>
/// Represents the outcome of an operation that returns a value of type <typeparamref name="TValue"/>.
/// </summary>
/// <typeparam name="TValue">The type of the value returned on success.</typeparam>
public readonly struct Result<TValue>
{
    /// <summary>
    /// The value associated with a successful result.
    /// </summary>
    private readonly TValue? _value;

    /// <summary>
    /// The error associated with a failed result.
    /// </summary>
    private readonly Error? _error;

    /// <summary>
    /// Gets a value indicating whether the result represents a successful outcome.
    /// </summary>
    public bool IsSuccess => _error is null;

    /// <summary>
    /// Gets a value indicating whether the result represents a failed outcome.
    /// </summary>
    public bool IsFailure => _error is not null;

    /// <summary>
    /// Gets the value associated with a successful result.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when accessed on a failed result.</exception>
    public TValue Value => IsSuccess ? _value! : throw new InvalidOperationException("Cannot access Value when Result is a failure.");

    /// <summary>
    /// Gets the error associated with a failed result.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when accessed on a successful result.</exception>
    public Error Error => IsFailure ? _error! : throw new InvalidOperationException("Cannot access Error when Result is successful.");

    /// <summary>
    /// Initializes a new instance of the <see cref="Result{TValue}"/> struct with the specified value.
    /// </summary>
    /// <param name="value">The value representing a successful outcome.</param>
    private Result(TValue value)
    {
        _value = value;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Result{TValue}"/> struct with the specified error.
    /// </summary>
    /// <param name="error">The error representing the failure.</param>
    private Result(Error error)
    {
        _error = error;
    }

    /// <summary>
    /// Creates a successful result with the specified value.
    /// </summary>
    /// <param name="value">The value representing the successful outcome.</param>
    /// <returns>A successful <see cref="Result{TValue}"/>.</returns>
    public static Result<TValue> Success(TValue value) => new(value);

    /// <summary>
    /// Creates a failed result with the specified error.
    /// </summary>
    /// <param name="error">The error representing the failure.</param>
    /// <returns>A failed <see cref="Result{TValue}"/>.</returns>
    public static Result<TValue> Failure(Error error) => new(error);

    /// <summary>
    /// Implicitly converts a value of type <typeparamref name="TValue"/> to a successful <see cref="Result{TValue}"/>.
    /// </summary>
    /// <param name="value">The value to convert.</param>
    public static implicit operator Result<TValue>(TValue value) => Success(value);

    /// <summary>
    /// Implicitly converts an <see cref="Error"/> to a failed <see cref="Result{TValue}"/>.
    /// </summary>
    /// <param name="error">The error to convert.</param>
    public static implicit operator Result<TValue>(Error error) => Failure(error);
}