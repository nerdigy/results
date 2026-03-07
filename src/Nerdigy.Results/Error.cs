namespace Nerdigy.Results;

/// <summary>
/// Represents a base error.
/// </summary>
public record Error;

/// <summary>
/// Represents a bad request error indicating the request was malformed or invalid.
/// </summary>
/// <param name="Message">The error message describing what was invalid.</param>
public record BadRequestError(string Message) : Error;

/// <summary>
/// Represents a validation error containing field-level validation failures.
/// </summary>
/// <param name="PropertyName">The name of the property that failed validation.</param>
/// <param name="ErrorMessage">The error message describing the validation failure.</param>
public record ValidationError(string PropertyName, string ErrorMessage) : Error;

/// <summary>
/// Represents an unauthorized error indicating the request lacks valid authentication credentials.
/// </summary>
/// <param name="Message">The error message describing the authentication failure.</param>
public record UnauthorizedError(string Message = "Unauthorized") : Error;

/// <summary>
/// Represents a forbidden error indicating the requester does not have permission to perform the action.
/// </summary>
/// <param name="Message">The error message describing the authorization failure.</param>
public record ForbiddenError(string Message = "Forbidden") : Error;

/// <summary>
/// Represents a not found error indicating the requested resource does not exist.
/// </summary>
/// <param name="Message">The error message describing what was not found.</param>
public record NotFoundError(string Message) : Error;

/// <summary>
/// Represents a conflict error indicating the request conflicts with the current state of the resource.
/// </summary>
/// <param name="Message">The error message describing the conflict.</param>
public record ConflictError(string Message) : Error;

/// <summary>
/// Represents an unprocessable error indicating the request was well-formed but contained semantic errors.
/// </summary>
/// <param name="Message">The error message describing the validation failure.</param>
public record UnprocessableError(string Message) : Error;

/// <summary>
/// Represents a rate limiting error indicating too many requests have been made.
/// </summary>
/// <param name="Message">The error message describing the rate limit.</param>
public record TooManyRequestsError(string Message = "Too many requests") : Error;

/// <summary>
/// Represents an internal error indicating an unexpected failure occurred.
/// </summary>
/// <param name="Message">The error message describing the failure.</param>
public record InternalError(string Message = "An internal error occurred") : Error;

/// <summary>
/// Represents a service unavailable error indicating the service is temporarily unable to handle the request.
/// </summary>
/// <param name="Message">The error message describing the unavailability.</param>
public record ServiceUnavailableError(string Message = "Service unavailable") : Error;

/// <summary>
/// Represents a gateway timeout error indicating an upstream service did not respond in time.
/// </summary>
/// <param name="Message">The error message describing the timeout.</param>
public record GatewayTimeoutError(string Message = "Gateway timeout") : Error;