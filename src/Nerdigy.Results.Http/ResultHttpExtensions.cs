using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Nerdigy.Results.Http;

/// <summary>
/// Provides extension methods for converting <see cref="Result"/> and <see cref="Result{TValue}"/> to ASP.NET Core <see cref="IResult"/> HTTP responses.
/// </summary>
public static class ResultHttpExtensions
{
    /// <summary>
    /// Provides extension methods for converting <see cref="Result"/> to <see cref="IResult"/>.
    /// </summary>
    extension(Result result)
    {
        /// <summary>
        /// Converts the result to an HTTP response.
        /// Returns 204 No Content on success, or a Problem Details response on failure.
        /// </summary>
        /// <returns>An <see cref="IResult"/> representing the HTTP response.</returns>
        public IResult AsHttpResult()
        {
            return result.Match<IResult>(TypedResults.NoContent, e => e.AsProblem());
        }

        /// <summary>
        /// Converts the result to an HTTP response using a custom success result factory.
        /// </summary>
        /// <param name="onSuccess">Factory to produce the HTTP result on success.</param>
        /// <returns>An <see cref="IResult"/> representing the HTTP response.</returns>
        public IResult AsHttpResult(Func<IResult> onSuccess)
        {
            return result.Match<IResult>(onSuccess, e => e.AsProblem());
        }
    }

    /// <summary>
    /// Provides extension methods for converting <see cref="Result{TValue}"/> to <see cref="IResult"/>.
    /// </summary>
    extension<TValue>(Result<TValue> result)
    {
        /// <summary>
        /// Converts the result to an HTTP response.
        /// Returns 200 OK with the value on success, or a Problem Details response on failure.
        /// </summary>
        /// <returns>An <see cref="IResult"/> representing the HTTP response.</returns>
        public IResult AsHttpResult()
        {
            return result.Match(TypedResults.Ok, e => e.AsProblem());
        }

        /// <summary>
        /// Converts the result to an HTTP response using a custom success result factory.
        /// </summary>
        /// <param name="onSuccess">Factory to produce the HTTP result from the value on success.</param>
        /// <returns>An <see cref="IResult"/> representing the HTTP response.</returns>
        public IResult AsHttpResult(Func<TValue, IResult> onSuccess)
        {
            return result.Match(onSuccess, e => e.AsProblem());
        }
    }

    /// <summary>
    /// Converts the specified <see cref="Error"/> to an <see cref="IResult"/> containing Problem Details.
    /// </summary>
    /// <param name="error">The error to convert.</param>
    /// <returns>An <see cref="IResult"/> with the appropriate HTTP status code and Problem Details body.</returns>
    public static IResult AsProblem(this Error error)
    {
        return error switch
        {
            ValidationError e => CreateValidationProblem(e),
            BadRequestError e => CreateProblem(StatusCodes.Status400BadRequest, "Bad Request", e.Message),
            UnauthorizedError e => CreateProblem(StatusCodes.Status401Unauthorized, "Unauthorized", e.Message),
            ForbiddenError e => CreateProblem(StatusCodes.Status403Forbidden, "Forbidden", e.Message),
            NotFoundError e => CreateProblem(StatusCodes.Status404NotFound, "Not Found", e.Message),
            ConflictError e => CreateProblem(StatusCodes.Status409Conflict, "Conflict", e.Message),
            UnprocessableError e => CreateProblem(StatusCodes.Status422UnprocessableEntity, "Unprocessable Entity", e.Message),
            TooManyRequestsError e => CreateProblem(StatusCodes.Status429TooManyRequests, "Too Many Requests", e.Message),
            InternalError e => CreateProblem(StatusCodes.Status500InternalServerError, "Internal Server Error", e.Message),
            ServiceUnavailableError e => CreateProblem(StatusCodes.Status503ServiceUnavailable, "Service Unavailable", e.Message),
            GatewayTimeoutError e => CreateProblem(StatusCodes.Status504GatewayTimeout, "Gateway Timeout", e.Message),
            _ => CreateProblem(StatusCodes.Status500InternalServerError, "Internal Server Error",
                "An unexpected error occurred"),
        };
    }

    /// <summary>
    /// Creates a Problem Details <see cref="IResult"/> with the specified status code, title, and detail.
    /// </summary>
    /// <param name="statusCode">The HTTP status code.</param>
    /// <param name="title">The short, human-readable summary of the problem type.</param>
    /// <param name="detail">The human-readable explanation specific to this occurrence of the problem.</param>
    /// <returns>An <see cref="ProblemHttpResult"/> containing the Problem Details response.</returns>
    private static ProblemHttpResult CreateProblem(int statusCode, string title, string detail)
    {
        return TypedResults.Problem(
            detail: detail,
            statusCode: statusCode,
            title: title);
    }

    /// <summary>
    /// Creates a Validation Problem Details <see cref="IResult"/> from the specified <see cref="ValidationError"/>.
    /// </summary>
    /// <param name="validation">The validation error containing the property name and error message.</param>
    /// <returns>An <see cref="ValidationProblem"/> containing the Validation Problem Details response.</returns>
    private static ValidationProblem CreateValidationProblem(ValidationError validation)
    {
        var errors = new Dictionary<string, string[]>
        {
            [validation.PropertyName] = [validation.ErrorMessage],
        };

        return TypedResults.ValidationProblem(errors);
    }
}