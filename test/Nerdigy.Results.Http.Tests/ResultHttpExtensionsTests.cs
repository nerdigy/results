using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Nerdigy.Results.Http.Tests;

public class ResultHttpExtensionsTests
{
    [Fact]
    public void AsHttpResult_Success_ReturnsNoContent()
    {
        var result = Result.Success;

        var httpResult = result.AsHttpResult();

        Assert.IsType<NoContent>(httpResult);
    }

    [Fact]
    public void AsHttpResult_Success_WithCustomFactory_ReturnsCustomResult()
    {
        var result = Result.Success;

        var httpResult = result.AsHttpResult(() => TypedResults.Ok("custom"));

        var okResult = Assert.IsType<Ok<string>>(httpResult);
        Assert.Equal("custom", okResult.Value);
    }

    [Fact]
    public void AsHttpResult_Failure_WithCustomFactory_ReturnsProblem()
    {
        Result result = new NotFoundError("Not found");

        var httpResult = result.AsHttpResult(() => TypedResults.Ok());

        var notFoundResult = Assert.IsType<NotFound<string>>(httpResult);
        Assert.Equal(StatusCodes.Status404NotFound, notFoundResult.StatusCode);
    }

    [Fact]
    public void AsHttpResult_BadRequestError_Returns400()
    {
        Result result = new BadRequestError("Invalid input");

        var httpResult = result.AsHttpResult();

        var badRequestResult = Assert.IsType<BadRequest<string>>(httpResult);
        Assert.Equal(StatusCodes.Status400BadRequest, badRequestResult.StatusCode);
        Assert.Equal("Invalid input", badRequestResult.Value);
    }

    [Fact]
    public void AsHttpResult_ValidationError_ReturnsValidationProblem()
    {
        Result result = new ValidationError(new Dictionary<string, string[]>
        {
            ["Email"] = ["Email is required"],
        });

        var httpResult = result.AsHttpResult();

        var validationResult = Assert.IsType<ValidationProblem>(httpResult);
        Assert.Equal(StatusCodes.Status400BadRequest, validationResult.StatusCode);
        Assert.Contains("Email", validationResult.ProblemDetails.Errors.Keys);
        Assert.Contains("Email is required", validationResult.ProblemDetails.Errors["Email"]);
    }

    [Fact]
    public void AsHttpResult_UnauthorizedError_Returns401()
    {
        Result result = new UnauthorizedError();

        var httpResult = result.AsHttpResult();

        var unauthorizedResult = Assert.IsType<UnauthorizedHttpResult>(httpResult);
        Assert.Equal(StatusCodes.Status401Unauthorized, unauthorizedResult.StatusCode);
    }

    [Fact]
    public void AsHttpResult_ForbiddenError_Returns403()
    {
        Result result = new ForbiddenError();

        var httpResult = result.AsHttpResult();

        Assert.IsType<ForbidHttpResult>(httpResult);
    }

    [Fact]
    public void AsHttpResult_NotFoundError_Returns404()
    {
        Result result = new NotFoundError("User not found");

        var httpResult = result.AsHttpResult();

        var notFoundResult = Assert.IsType<NotFound<string>>(httpResult);
        Assert.Equal(StatusCodes.Status404NotFound, notFoundResult.StatusCode);
        Assert.Equal("User not found", notFoundResult.Value);
    }

    [Fact]
    public void AsHttpResult_ConflictError_Returns409()
    {
        Result result = new ConflictError("Already exists");

        var httpResult = result.AsHttpResult();

        var conflictResult = Assert.IsType<Conflict<string>>(httpResult);
        Assert.Equal(StatusCodes.Status409Conflict, conflictResult.StatusCode);
        Assert.Equal("Already exists", conflictResult.Value);
    }

    [Fact]
    public void AsHttpResult_UnprocessableError_Returns422()
    {
        Result result = new UnprocessableError("Semantic error");

        var httpResult = result.AsHttpResult();

        var unprocessableResult = Assert.IsType<UnprocessableEntity<string>>(httpResult);
        Assert.Equal(StatusCodes.Status422UnprocessableEntity, unprocessableResult.StatusCode);
        Assert.Equal("Semantic error", unprocessableResult.Value);
    }

    [Fact]
    public void AsHttpResult_InternalError_Returns500()
    {
        Result result = new InternalError();

        var httpResult = result.AsHttpResult();

        var internalResult = Assert.IsType<InternalServerError<string>>(httpResult);
        Assert.Equal(StatusCodes.Status500InternalServerError, internalResult.StatusCode);
    }

    [Fact]
    public void AsHttpResult_BaseError_Returns500()
    {
        Result result = new Error();

        var httpResult = result.AsHttpResult();

        var internalResult = Assert.IsType<InternalServerError>(httpResult);
        Assert.Equal(StatusCodes.Status500InternalServerError, internalResult.StatusCode);
    }
}
