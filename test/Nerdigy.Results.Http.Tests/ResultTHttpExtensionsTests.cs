using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Nerdigy.Results.Http.Tests;

public class ResultTHttpExtensionsTests
{
    [Fact]
    public void ToHttpResult_Success_ReturnsOkWithValue()
    {
        Result<string> result = "hello";

        var httpResult = result.ToHttpResult();

        var okResult = Assert.IsType<Ok<string>>(httpResult);
        Assert.Equal("hello", okResult.Value);
    }

    [Fact]
    public void ToHttpResult_Success_WithComplexType_ReturnsOkWithValue()
    {
        var item = new TestItem(1, "Test");
        Result<TestItem> result = item;

        var httpResult = result.ToHttpResult();

        var okResult = Assert.IsType<Ok<TestItem>>(httpResult);
        Assert.Equal(item, okResult.Value);
    }

    [Fact]
    public void ToHttpResult_Success_WithCustomFactory_ReturnsCustomResult()
    {
        Result<string> result = "hello";

        var httpResult = result.ToHttpResult(value => TypedResults.Created($"/items/{value}", value));

        var createdResult = Assert.IsType<Created<string>>(httpResult);
        Assert.Equal("hello", createdResult.Value);
        Assert.Equal("/items/hello", createdResult.Location);
    }

    [Fact]
    public void ToHttpResult_Failure_WithCustomFactory_ReturnsProblemDetails()
    {
        Result<string> result = new NotFoundError("Not found");

        var httpResult = result.ToHttpResult(value => TypedResults.Ok(value));

        var problemResult = Assert.IsType<ProblemHttpResult>(httpResult);
        Assert.Equal(StatusCodes.Status404NotFound, problemResult.StatusCode);
    }

    [Fact]
    public void ToHttpResult_BadRequestError_Returns400()
    {
        Result<string> result = new BadRequestError("Invalid");

        var httpResult = result.ToHttpResult();

        var problemResult = Assert.IsType<ProblemHttpResult>(httpResult);
        Assert.Equal(StatusCodes.Status400BadRequest, problemResult.StatusCode);
        Assert.Equal("Bad Request", problemResult.ProblemDetails.Title);
        Assert.Equal("Invalid", problemResult.ProblemDetails.Detail);
    }

    [Fact]
    public void ToHttpResult_ValidationError_ReturnsValidationProblem()
    {
        Result<string> result = new ValidationError("Name", "Name is required");

        var httpResult = result.ToHttpResult();

        var validationResult = Assert.IsType<ValidationProblem>(httpResult);
        Assert.Equal(StatusCodes.Status400BadRequest, validationResult.StatusCode);
        Assert.Contains("Name", validationResult.ProblemDetails.Errors.Keys);
        Assert.Contains("Name is required", validationResult.ProblemDetails.Errors["Name"]);
    }

    [Fact]
    public void ToHttpResult_NotFoundError_Returns404()
    {
        Result<int> result = new NotFoundError("Item not found");

        var httpResult = result.ToHttpResult();

        var problemResult = Assert.IsType<ProblemHttpResult>(httpResult);
        Assert.Equal(StatusCodes.Status404NotFound, problemResult.StatusCode);
        Assert.Equal("Item not found", problemResult.ProblemDetails.Detail);
    }

    [Fact]
    public void ToHttpResult_InternalError_Returns500()
    {
        Result<string> result = new InternalError();

        var httpResult = result.ToHttpResult();

        var problemResult = Assert.IsType<ProblemHttpResult>(httpResult);
        Assert.Equal(StatusCodes.Status500InternalServerError, problemResult.StatusCode);
    }

    [Fact]
    public void ToHttpResult_BaseError_Returns500()
    {
        Result<string> result = new Error();

        var httpResult = result.ToHttpResult();

        var problemResult = Assert.IsType<ProblemHttpResult>(httpResult);
        Assert.Equal(StatusCodes.Status500InternalServerError, problemResult.StatusCode);
        Assert.Equal("An unexpected error occurred", problemResult.ProblemDetails.Detail);
    }

    private record TestItem(int Id, string Name);
}
