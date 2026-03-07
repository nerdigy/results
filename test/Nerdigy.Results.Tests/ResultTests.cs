namespace Nerdigy.Results.Tests;

public sealed class ResultTests
{
    [Fact]
    public void Success_IsSuccess_ReturnsTrue()
    {
        var result = Result.Success;

        Assert.True(result.IsSuccess);
        Assert.False(result.IsFailure);
    }

    [Fact]
    public void Failure_IsFailure_ReturnsTrue()
    {
        var result = Result.Failure(new BadRequestError("fail"));

        Assert.True(result.IsFailure);
        Assert.False(result.IsSuccess);
    }

    [Fact]
    public void Failure_Error_ReturnsError()
    {
        var error = new BadRequestError("fail");
        var result = Result.Failure(error);

        Assert.Equal(error, result.Error);
    }

    [Fact]
    public void Success_Error_ThrowsInvalidOperationException()
    {
        var result = Result.Success;

        Assert.Throws<InvalidOperationException>(() => result.Error);
    }

    [Fact]
    public void ImplicitConversion_FromError_CreatesFailure()
    {
        Result result = new BadRequestError("fail");

        Assert.True(result.IsFailure);
    }

    [Fact]
    public void Match_OnSuccess_InvokesOnSuccess()
    {
        var result = Result.Success;

        var value = result.Match(() => "success", _ => "failure");

        Assert.Equal("success", value);
    }

    [Fact]
    public void Match_OnFailure_InvokesOnFailure()
    {
        var error = new BadRequestError("fail");
        Result result = error;

        var value = result.Match(() => "success", e => ((BadRequestError)e).Message);

        Assert.Equal("fail", value);
    }

    [Fact]
    public void Bind_OnSuccess_ReturnsChainedResult()
    {
        var result = Result.Success;

        var bound = result.Bind(() => Result.Success);

        Assert.True(bound.IsSuccess);
    }

    [Fact]
    public void Bind_OnFailure_ReturnsOriginalFailure()
    {
        var error = new BadRequestError("fail");
        Result result = error;

        var bound = result.Bind(() => Result.Success);

        Assert.True(bound.IsFailure);
        Assert.Equal(error, bound.Error);
    }

    [Fact]
    public void BindGeneric_OnSuccess_ReturnsChainedResult()
    {
        var result = Result.Success;

        var bound = result.Bind(() => Result<int>.Success(42));

        Assert.True(bound.IsSuccess);
        Assert.Equal(42, bound.Value);
    }

    [Fact]
    public void BindGeneric_OnFailure_ReturnsFailureWithOriginalError()
    {
        var error = new BadRequestError("fail");
        Result result = error;

        var bound = result.Bind(() => Result<int>.Success(42));

        Assert.True(bound.IsFailure);
        Assert.Equal(error, bound.Error);
    }

    [Fact]
    public void Tap_OnSuccess_InvokesAction()
    {
        var result = Result.Success;
        var tapped = false;

        var returned = result.Tap(() => tapped = true);

        Assert.True(tapped);
        Assert.True(returned.IsSuccess);
    }

    [Fact]
    public void Tap_OnFailure_DoesNotInvokeAction()
    {
        Result result = new BadRequestError("fail");
        var tapped = false;

        var returned = result.Tap(() => tapped = true);

        Assert.False(tapped);
        Assert.True(returned.IsFailure);
    }

    [Fact]
    public async Task MatchAsync_OnSuccess_InvokesOnSuccess()
    {
        var result = Result.Success;

        var value = await result.MatchAsync(
            () => Task.FromResult("success"),
            _ => Task.FromResult("failure"));

        Assert.Equal("success", value);
    }

    [Fact]
    public async Task MatchAsync_OnFailure_InvokesOnFailure()
    {
        Result result = new BadRequestError("fail");

        var value = await result.MatchAsync(
            () => Task.FromResult("success"),
            e => Task.FromResult(((BadRequestError)e).Message));

        Assert.Equal("fail", value);
    }

    [Fact]
    public async Task BindAsync_OnSuccess_ReturnsChainedResult()
    {
        var result = Result.Success;

        var bound = await result.BindAsync(() => Task.FromResult(Result.Success));

        Assert.True(bound.IsSuccess);
    }

    [Fact]
    public async Task BindAsync_OnFailure_ReturnsOriginalFailure()
    {
        var error = new BadRequestError("fail");
        Result result = error;

        var bound = await result.BindAsync(() => Task.FromResult(Result.Success));

        Assert.True(bound.IsFailure);
        Assert.Equal(error, bound.Error);
    }

    [Fact]
    public async Task BindAsyncGeneric_OnSuccess_ReturnsChainedResult()
    {
        var result = Result.Success;

        var bound = await result.BindAsync(() => Task.FromResult(Result<int>.Success(42)));

        Assert.True(bound.IsSuccess);
        Assert.Equal(42, bound.Value);
    }

    [Fact]
    public async Task BindAsyncGeneric_OnFailure_ReturnsFailureWithOriginalError()
    {
        var error = new BadRequestError("fail");
        Result result = error;

        var bound = await result.BindAsync(() => Task.FromResult(Result<int>.Success(42)));

        Assert.True(bound.IsFailure);
        Assert.Equal(error, bound.Error);
    }

    [Fact]
    public async Task TapAsync_OnSuccess_InvokesAction()
    {
        var result = Result.Success;
        var tapped = false;

        var returned = await result.TapAsync(() =>
        {
            tapped = true;

            return Task.CompletedTask;
        });

        Assert.True(tapped);
        Assert.True(returned.IsSuccess);
    }

    [Fact]
    public async Task TapAsync_OnFailure_DoesNotInvokeAction()
    {
        Result result = new BadRequestError("fail");
        var tapped = false;

        var returned = await result.TapAsync(() =>
        {
            tapped = true;

            return Task.CompletedTask;
        });

        Assert.False(tapped);
        Assert.True(returned.IsFailure);
    }
}
