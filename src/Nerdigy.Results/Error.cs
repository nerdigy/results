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
/// Represents a validation error indicating the request failed validation checks.
/// </summary>
/// <param name="Details">A dictionary containing the validation errors, where the key is the property name and the value is an array of error messages.</param>
public record ValidationError(Dictionary<string, string[]> Details) : Error;

/// <summary>
/// Represents an unauthorized error indicating the request lacks valid authentication credentials.
/// </summary>
public record UnauthorizedError : Error;

/// <summary>
/// Represents a forbidden error indicating the requester does not have permission to perform the action.
/// </summary>
public record ForbiddenError : Error;

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
/// Represents an internal error indicating an unexpected failure occurred.
/// </summary>
/// <param name="Message">The error message describing the failure.</param>
public record InternalError(string Message = "An internal error occurred") : Error;