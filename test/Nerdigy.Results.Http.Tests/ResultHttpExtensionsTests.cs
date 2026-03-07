using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Nerdigy.Results.Http.Tests;

public class ResultHttpExtensionsTests
{
    [Fact]
    public void ToHttpResult_Success_ReturnsNoContent()
    {
        var result = Result.Success;

        var httpResult = result.ToHttpResult();

        Assert.IsType<NoContent>(httpResult);
    }

    [Fact]
    public void ToHttpResult_Success_WithCustomFactory_ReturnsCustomResult()
    {
        var result = Result.Success;

        var httpResult = result.ToHttpResult(() => TypedResults.Ok("custom"));

        var okResult = Assert.IsType<Ok<string>>(httpResult);
        Assert.Equal("custom", okResult.Value);
    }

    [Fact]
    public void ToHttpResult_Failure_WithCustomFactory_ReturnsProblemDetails()
    {
        Result result = new NotFoundError("Not found");

        var httpResult = result.ToHttpResult(() => TypedResults.Ok());

        var problemResult = Assert.IsType<ProblemHttpResult>(httpResult);
        Assert.Equal(StatusCodes.Status404NotFound, problemResult.StatusCode);
    }

    [Fact]
    public void ToHttpResult_BadRequestError_Returns400()
    {
        Result result = new BadRequestError("Invalid input");

        var httpResult = result.ToHttpResult();

        var problemResult = Assert.IsType<ProblemHttpResult>(httpResult);
        Assert.Equal(StatusCodes.Status400BadRequest, problemResult.StatusCode);
        Assert.Equal("Bad Request", problemResult.ProblemDetails.Title);
        Assert.Equal("Invalid input", problemResult.ProblemDetails.Detail);
    }

    [Fact]
    public void ToHttpResult_ValidationError_ReturnsValidationProblem()
    {
        Result result = new ValidationError("Email", "Email is required");

        var httpResult = result.ToHttpResult();

        var validationResult = Assert.IsType<ValidationProblem>(httpResult);
        Assert.Equal(StatusCodes.Status400BadRequest, validationResult.StatusCode);
        Assert.Contains("Email", validationResult.ProblemDetails.Errors.Keys);
        Assert.Contains("Email is required", validationResult.ProblemDetails.Errors["Email"]);
    }

    [Fact]
    public void ToHttpResult_UnauthorizedError_Returns401()
    {
        Result result = new UnauthorizedError();

        var httpResult = result.ToHttpResult();

        var problemResult = Assert.IsType<ProblemHttpResult>(httpResult);
        Assert.Equal(StatusCodes.Status401Unauthorized, problemResult.StatusCode);
        Assert.Equal("Unauthorized", problemResult.ProblemDetails.Title);
    }

    [Fact]
    public void ToHttpResult_ForbiddenError_Returns403()
    {
        Result result = new ForbiddenError();

        var httpResult = result.ToHttpResult();

        var problemResult = Assert.IsType<ProblemHttpResult>(httpResult);
        Assert.Equal(StatusCodes.Status403Forbidden, problemResult.StatusCode);
        Assert.Equal("Forbidden", problemResult.ProblemDetails.Title);
    }

    [Fact]
    public void ToHttpResult_NotFoundError_Returns404()
    {
        Result result = new NotFoundError("User not found");

        var httpResult = result.ToHttpResult();

        var problemResult = Assert.IsType<ProblemHttpResult>(httpResult);
        Assert.Equal(StatusCodes.Status404NotFound, problemResult.StatusCode);
        Assert.Equal("Not Found", problemResult.ProblemDetails.Title);
        Assert.Equal("User not found", problemResult.ProblemDetails.Detail);
    }

    [Fact]
    public void ToHttpResult_ConflictError_Returns409()
    {
        Result result = new ConflictError("Already exists");

        var httpResult = result.ToHttpResult();

        var problemResult = Assert.IsType<ProblemHttpResult>(httpResult);
        Assert.Equal(StatusCodes.Status409Conflict, problemResult.StatusCode);
        Assert.Equal("Conflict", problemResult.ProblemDetails.Title);
    }

    [Fact]
    public void ToHttpResult_UnprocessableError_Returns422()
    {
        Result result = new UnprocessableError("Semantic error");

        var httpResult = result.ToHttpResult();

        var problemResult = Assert.IsType<ProblemHttpResult>(httpResult);
        Assert.Equal(StatusCodes.Status422UnprocessableEntity, problemResult.StatusCode);
        Assert.Equal("Unprocessable Entity", problemResult.ProblemDetails.Title);
    }

    [Fact]
    public void ToHttpResult_TooManyRequestsError_Returns429()
    {
        Result result = new TooManyRequestsError();

        var httpResult = result.ToHttpResult();

        var problemResult = Assert.IsType<ProblemHttpResult>(httpResult);
        Assert.Equal(StatusCodes.Status429TooManyRequests, problemResult.StatusCode);
        Assert.Equal("Too Many Requests", problemResult.ProblemDetails.Title);
    }

    [Fact]
    public void ToHttpResult_InternalError_Returns500()
    {
        Result result = new InternalError();

        var httpResult = result.ToHttpResult();

        var problemResult = Assert.IsType<ProblemHttpResult>(httpResult);
        Assert.Equal(StatusCodes.Status500InternalServerError, problemResult.StatusCode);
        Assert.Equal("Internal Server Error", problemResult.ProblemDetails.Title);
    }

    [Fact]
    public void ToHttpResult_ServiceUnavailableError_Returns503()
    {
        Result result = new ServiceUnavailableError();

        var httpResult = result.ToHttpResult();

        var problemResult = Assert.IsType<ProblemHttpResult>(httpResult);
        Assert.Equal(StatusCodes.Status503ServiceUnavailable, problemResult.StatusCode);
        Assert.Equal("Service Unavailable", problemResult.ProblemDetails.Title);
    }

    [Fact]
    public void ToHttpResult_GatewayTimeoutError_Returns504()
    {
        Result result = new GatewayTimeoutError();

        var httpResult = result.ToHttpResult();

        var problemResult = Assert.IsType<ProblemHttpResult>(httpResult);
        Assert.Equal(StatusCodes.Status504GatewayTimeout, problemResult.StatusCode);
        Assert.Equal("Gateway Timeout", problemResult.ProblemDetails.Title);
    }

    [Fact]
    public void ToHttpResult_BaseError_Returns500()
    {
        Result result = new Error();

        var httpResult = result.ToHttpResult();

        var problemResult = Assert.IsType<ProblemHttpResult>(httpResult);
        Assert.Equal(StatusCodes.Status500InternalServerError, problemResult.StatusCode);
        Assert.Equal("An unexpected error occurred", problemResult.ProblemDetails.Detail);
    }
}