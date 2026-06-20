namespace Nerdigy.Results.Tests;

/// <summary>
/// Tests for the extension methods whose receiver is a <see cref="Task{Result}"/>,
/// enabling chaining directly off an asynchronous operation.
/// </summary>
public sealed class TaskResultExtensionsTests
{
    private static Task<Result> SuccessAsync() => Task.FromResult(Result.Success);

    private static Task<Result> FailureAsync(Error error) => Task.FromResult(Result.Failure(error));

    [Fact]
    public async Task MatchAsync_Sync_OnSuccess_InvokesOnSuccess()
    {
        var value = await SuccessAsync().MatchAsync(() => "success", _ => "failure");

        Assert.Equal("success", value);
    }

    [Fact]
    public async Task MatchAsync_Sync_OnFailure_InvokesOnFailure()
    {
        var value = await FailureAsync(new BadRequestError("fail"))
            .MatchAsync(() => "success", e => ((BadRequestError)e).Message);

        Assert.Equal("fail", value);
    }

    [Fact]
    public async Task MatchAsync_Async_OnSuccess_InvokesOnSuccess()
    {
        var value = await SuccessAsync().MatchAsync(
            () => Task.FromResult("success"),
            _ => Task.FromResult("failure"));

        Assert.Equal("success", value);
    }

    [Fact]
    public async Task MatchAsync_Async_OnFailure_InvokesOnFailure()
    {
        var value = await FailureAsync(new BadRequestError("fail")).MatchAsync(
            () => Task.FromResult("success"),
            e => Task.FromResult(((BadRequestError)e).Message));

        Assert.Equal("fail", value);
    }

    [Fact]
    public async Task BindAsync_Sync_OnSuccess_ReturnsChainedResult()
    {
        var bound = await SuccessAsync().BindAsync(() => Result.Success);

        Assert.True(bound.IsSuccess);
    }

    [Fact]
    public async Task BindAsync_Sync_OnFailure_ReturnsOriginalFailure()
    {
        var error = new BadRequestError("fail");

        var bound = await FailureAsync(error).BindAsync(() => Result.Success);

        Assert.True(bound.IsFailure);
        Assert.Equal(error, bound.Error);
    }

    [Fact]
    public async Task BindAsync_Async_OnSuccess_ReturnsChainedResult()
    {
        var bound = await SuccessAsync().BindAsync(() => Task.FromResult(Result.Success));

        Assert.True(bound.IsSuccess);
    }

    [Fact]
    public async Task BindAsync_Async_OnFailure_ReturnsOriginalFailure()
    {
        var error = new BadRequestError("fail");

        var bound = await FailureAsync(error).BindAsync(() => Task.FromResult(Result.Success));

        Assert.True(bound.IsFailure);
        Assert.Equal(error, bound.Error);
    }

    [Fact]
    public async Task BindAsyncGeneric_Sync_OnSuccess_ReturnsChainedResult()
    {
        var bound = await SuccessAsync().BindAsync(() => Result<int>.Success(42));

        Assert.True(bound.IsSuccess);
        Assert.Equal(42, bound.Value);
    }

    [Fact]
    public async Task BindAsyncGeneric_Sync_OnFailure_ReturnsFailureWithOriginalError()
    {
        var error = new BadRequestError("fail");

        var bound = await FailureAsync(error).BindAsync(() => Result<int>.Success(42));

        Assert.True(bound.IsFailure);
        Assert.Equal(error, bound.Error);
    }

    [Fact]
    public async Task BindAsyncGeneric_Async_OnSuccess_ReturnsChainedResult()
    {
        var bound = await SuccessAsync().BindAsync(() => Task.FromResult(Result<int>.Success(42)));

        Assert.True(bound.IsSuccess);
        Assert.Equal(42, bound.Value);
    }

    [Fact]
    public async Task BindAsyncGeneric_Async_OnFailure_ReturnsFailureWithOriginalError()
    {
        var error = new BadRequestError("fail");

        var bound = await FailureAsync(error).BindAsync(() => Task.FromResult(Result<int>.Success(42)));

        Assert.True(bound.IsFailure);
        Assert.Equal(error, bound.Error);
    }

    [Fact]
    public async Task TapAsync_Sync_OnSuccess_InvokesAction()
    {
        var tapped = false;

        var returned = await SuccessAsync().TapAsync(() => tapped = true);

        Assert.True(tapped);
        Assert.True(returned.IsSuccess);
    }

    [Fact]
    public async Task TapAsync_Sync_OnFailure_DoesNotInvokeAction()
    {
        var tapped = false;

        var returned = await FailureAsync(new BadRequestError("fail")).TapAsync(() => tapped = true);

        Assert.False(tapped);
        Assert.True(returned.IsFailure);
    }

    [Fact]
    public async Task TapAsync_Async_OnSuccess_InvokesAction()
    {
        var tapped = false;

        var returned = await SuccessAsync().TapAsync(() =>
        {
            tapped = true;

            return Task.CompletedTask;
        });

        Assert.True(tapped);
        Assert.True(returned.IsSuccess);
    }

    [Fact]
    public async Task TapAsync_Async_OnFailure_DoesNotInvokeAction()
    {
        var tapped = false;

        var returned = await FailureAsync(new BadRequestError("fail")).TapAsync(() =>
        {
            tapped = true;

            return Task.CompletedTask;
        });

        Assert.False(tapped);
        Assert.True(returned.IsFailure);
    }

    [Fact]
    public async Task TapErrorAsync_Sync_OnFailure_InvokesAction()
    {
        var error = new BadRequestError("fail");
        Error? tappedError = null;

        var returned = await FailureAsync(error).TapErrorAsync(e => tappedError = e);

        Assert.Equal(error, tappedError);
        Assert.True(returned.IsFailure);
        Assert.Equal(error, returned.Error);
    }

    [Fact]
    public async Task TapErrorAsync_Sync_OnSuccess_DoesNotInvokeAction()
    {
        var tapped = false;

        var returned = await SuccessAsync().TapErrorAsync(_ => tapped = true);

        Assert.False(tapped);
        Assert.True(returned.IsSuccess);
    }

    [Fact]
    public async Task TapErrorAsync_Async_OnFailure_InvokesAction()
    {
        var error = new BadRequestError("fail");
        Error? tappedError = null;

        var returned = await FailureAsync(error).TapErrorAsync(e =>
        {
            tappedError = e;

            return Task.CompletedTask;
        });

        Assert.Equal(error, tappedError);
        Assert.True(returned.IsFailure);
        Assert.Equal(error, returned.Error);
    }

    [Fact]
    public async Task TapErrorAsync_Async_OnSuccess_DoesNotInvokeAction()
    {
        var tapped = false;

        var returned = await SuccessAsync().TapErrorAsync(_ =>
        {
            tapped = true;

            return Task.CompletedTask;
        });

        Assert.False(tapped);
        Assert.True(returned.IsSuccess);
    }

    [Fact]
    public async Task BindAsync_ChainsMultipleAsyncOperations()
    {
        // Chains directly off an async operation without an intermediate await.
        var bound = await SuccessAsync()
            .BindAsync(() => Task.FromResult(Result.Success))
            .BindAsync(() => Task.FromResult(Result<int>.Success(42)));

        Assert.True(bound.IsSuccess);
        Assert.Equal(42, bound.Value);
    }

    [Fact]
    public async Task BindAsync_ShortCircuitsChainOnFailure()
    {
        var error = new BadRequestError("fail");
        var secondInvoked = false;

        var bound = await FailureAsync(error)
            .BindAsync(() =>
            {
                secondInvoked = true;

                return Task.FromResult(Result.Success);
            });

        Assert.False(secondInvoked);
        Assert.True(bound.IsFailure);
        Assert.Equal(error, bound.Error);
    }
}