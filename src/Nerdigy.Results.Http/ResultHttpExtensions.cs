using Microsoft.AspNetCore.Http;

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
            ValidationError e => TypedResults.ValidationProblem(e.Details),
            BadRequestError e => TypedResults.BadRequest(e.Message),
            UnauthorizedError => TypedResults.Unauthorized(),
            ForbiddenError => TypedResults.Forbid(),
            NotFoundError e => TypedResults.NotFound(e.Message),
            ConflictError e => TypedResults.Conflict(e.Message),
            UnprocessableError e => TypedResults.UnprocessableEntity(e.Message),
            InternalError e => TypedResults.InternalServerError(e.Message),
            _ => TypedResults.InternalServerError()
        };
    }
}