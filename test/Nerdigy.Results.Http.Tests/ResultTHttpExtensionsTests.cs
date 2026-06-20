using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Nerdigy.Results.Http.Tests;

public class ResultTHttpExtensionsTests
{
    [Fact]
    public void AsHttpResult_Success_ReturnsOkWithValue()
    {
        Result<string> result = "hello";

        var httpResult = result.AsHttpResult();

        var okResult = Assert.IsType<Ok<string>>(httpResult);
        Assert.Equal("hello", okResult.Value);
    }

    [Fact]
    public void AsHttpResult_Success_WithComplexType_ReturnsOkWithValue()
    {
        var item = new TestItem(1, "Test");
        Result<TestItem> result = item;

        var httpResult = result.AsHttpResult();

        var okResult = Assert.IsType<Ok<TestItem>>(httpResult);
        Assert.Equal(item, okResult.Value);
    }

    [Fact]
    public void AsHttpResult_Success_WithCustomFactory_ReturnsCustomResult()
    {
        Result<string> result = "hello";

        var httpResult = result.AsHttpResult(value => TypedResults.Created($"/items/{value}", value));

        var createdResult = Assert.IsType<Created<string>>(httpResult);
        Assert.Equal("hello", createdResult.Value);
        Assert.Equal("/items/hello", createdResult.Location);
    }

    [Fact]
    public void AsHttpResult_Failure_WithCustomFactory_ReturnsProblem()
    {
        Result<string> result = new NotFoundError("Not found");

        var httpResult = result.AsHttpResult(value => TypedResults.Ok(value));

        var notFoundResult = Assert.IsType<NotFound<string>>(httpResult);
        Assert.Equal(StatusCodes.Status404NotFound, notFoundResult.StatusCode);
    }

    [Fact]
    public void AsHttpResult_BadRequestError_Returns400()
    {
        Result<string> result = new BadRequestError("Invalid");

        var httpResult = result.AsHttpResult();

        var badRequestResult = Assert.IsType<BadRequest<string>>(httpResult);
        Assert.Equal(StatusCodes.Status400BadRequest, badRequestResult.StatusCode);
        Assert.Equal("Invalid", badRequestResult.Value);
    }

    [Fact]
    public void AsHttpResult_ValidationError_ReturnsValidationProblem()
    {
        Result<string> result = new ValidationError(new Dictionary<string, string[]>
        {
            ["Name"] = ["Name is required"],
        });

        var httpResult = result.AsHttpResult();

        var validationResult = Assert.IsType<ValidationProblem>(httpResult);
        Assert.Equal(StatusCodes.Status400BadRequest, validationResult.StatusCode);
        Assert.Contains("Name", validationResult.ProblemDetails.Errors.Keys);
        Assert.Contains("Name is required", validationResult.ProblemDetails.Errors["Name"]);
    }

    [Fact]
    public void AsHttpResult_NotFoundError_Returns404()
    {
        Result<int> result = new NotFoundError("Item not found");

        var httpResult = result.AsHttpResult();

        var notFoundResult = Assert.IsType<NotFound<string>>(httpResult);
        Assert.Equal(StatusCodes.Status404NotFound, notFoundResult.StatusCode);
        Assert.Equal("Item not found", notFoundResult.Value);
    }

    [Fact]
    public void AsHttpResult_InternalError_Returns500()
    {
        Result<string> result = new InternalError();

        var httpResult = result.AsHttpResult();

        var internalResult = Assert.IsType<InternalServerError<string>>(httpResult);
        Assert.Equal(StatusCodes.Status500InternalServerError, internalResult.StatusCode);
    }

    [Fact]
    public void AsHttpResult_BaseError_Returns500()
    {
        Result<string> result = new Error();

        var httpResult = result.AsHttpResult();

        var internalResult = Assert.IsType<InternalServerError>(httpResult);
        Assert.Equal(StatusCodes.Status500InternalServerError, internalResult.StatusCode);
    }

    private record TestItem(int Id, string Name);
}