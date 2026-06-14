namespace Nerdigy.Results.Tests;

/// <summary>
/// Tests for the extension methods whose receiver is a <see cref="Task{TResult}"/> of
/// <see cref="Result{TValue}"/>, enabling chaining directly off an asynchronous operation.
/// </summary>
public sealed class TaskResultTExtensionsTests
{
    private static Task<Result<int>> SuccessAsync(int value = 42) => Task.FromResult(Result<int>.Success(value));

    private static Task<Result<int>> FailureAsync(Error error) => Task.FromResult(Result<int>.Failure(error));

    [Fact]
    public async Task MatchAsync_Sync_OnSuccess_InvokesOnSuccess()
    {
        var value = await SuccessAsync().MatchAsync(v => v.ToString(), _ => "failure");

        Assert.Equal("42", value);
    }

    [Fact]
    public async Task MatchAsync_Sync_OnFailure_InvokesOnFailure()
    {
        var value = await FailureAsync(new BadRequestError("fail"))
            .MatchAsync(v => v.ToString(), e => ((BadRequestError)e).Message);

        Assert.Equal("fail", value);
    }

    [Fact]
    public async Task MatchAsync_Async_OnSuccess_InvokesOnSuccess()
    {
        var value = await SuccessAsync().MatchAsync(
            v => Task.FromResult(v.ToString()),
            _ => Task.FromResult("failure"));

        Assert.Equal("42", value);
    }

    [Fact]
    public async Task MatchAsync_Async_OnFailure_InvokesOnFailure()
    {
        var value = await FailureAsync(new BadRequestError("fail")).MatchAsync(
            v => Task.FromResult(v.ToString()),
            e => Task.FromResult(((BadRequestError)e).Message));

        Assert.Equal("fail", value);
    }

    [Fact]
    public async Task MapAsync_Sync_OnSuccess_TransformsValue()
    {
        var mapped = await SuccessAsync().MapAsync(v => v * 2);

        Assert.True(mapped.IsSuccess);
        Assert.Equal(84, mapped.Value);
    }

    [Fact]
    public async Task MapAsync_Sync_OnFailure_ReturnsFailureWithOriginalError()
    {
        var error = new BadRequestError("fail");

        var mapped = await FailureAsync(error).MapAsync(v => v * 2);

        Assert.True(mapped.IsFailure);
        Assert.Equal(error, mapped.Error);
    }

    [Fact]
    public async Task MapAsync_Async_OnSuccess_TransformsValue()
    {
        var mapped = await SuccessAsync().MapAsync(v => Task.FromResult(v * 2));

        Assert.True(mapped.IsSuccess);
        Assert.Equal(84, mapped.Value);
    }

    [Fact]
    public async Task MapAsync_Async_OnFailure_ReturnsFailureWithOriginalError()
    {
        var error = new BadRequestError("fail");

        var mapped = await FailureAsync(error).MapAsync(v => Task.FromResult(v * 2));

        Assert.True(mapped.IsFailure);
        Assert.Equal(error, mapped.Error);
    }

    [Fact]
    public async Task BindAsyncGeneric_Sync_OnSuccess_ReturnsChainedResult()
    {
        var bound = await SuccessAsync().BindAsync(v => Result<string>.Success(v.ToString()));

        Assert.True(bound.IsSuccess);
        Assert.Equal("42", bound.Value);
    }

    [Fact]
    public async Task BindAsyncGeneric_Sync_OnFailure_ReturnsFailureWithOriginalError()
    {
        var error = new BadRequestError("fail");

        var bound = await FailureAsync(error).BindAsync(v => Result<string>.Success(v.ToString()));

        Assert.True(bound.IsFailure);
        Assert.Equal(error, bound.Error);
    }

    [Fact]
    public async Task BindAsyncGeneric_Async_OnSuccess_ReturnsChainedResult()
    {
        var bound = await SuccessAsync().BindAsync(v => Task.FromResult(Result<string>.Success(v.ToString())));

        Assert.True(bound.IsSuccess);
        Assert.Equal("42", bound.Value);
    }

    [Fact]
    public async Task BindAsyncGeneric_Async_OnFailure_ReturnsFailureWithOriginalError()
    {
        var error = new BadRequestError("fail");

        var bound = await FailureAsync(error).BindAsync(v => Task.FromResult(Result<string>.Success(v.ToString())));

        Assert.True(bound.IsFailure);
        Assert.Equal(error, bound.Error);
    }

    [Fact]
    public async Task BindAsync_Sync_OnSuccess_ReturnsChainedResult()
    {
        var bound = await SuccessAsync().BindAsync(_ => Result.Success);

        Assert.True(bound.IsSuccess);
    }

    [Fact]
    public async Task BindAsync_Sync_OnFailure_ReturnsFailureWithOriginalError()
    {
        var error = new BadRequestError("fail");

        var bound = await FailureAsync(error).BindAsync(_ => Result.Success);

        Assert.True(bound.IsFailure);
        Assert.Equal(error, bound.Error);
    }

    [Fact]
    public async Task BindAsync_Async_OnSuccess_ReturnsChainedResult()
    {
        var bound = await SuccessAsync().BindAsync(_ => Task.FromResult(Result.Success));

        Assert.True(bound.IsSuccess);
    }

    [Fact]
    public async Task BindAsync_Async_OnFailure_ReturnsFailureWithOriginalError()
    {
        var error = new BadRequestError("fail");

        var bound = await FailureAsync(error).BindAsync(_ => Task.FromResult(Result.Success));

        Assert.True(bound.IsFailure);
        Assert.Equal(error, bound.Error);
    }

    [Fact]
    public async Task TapAsync_Sync_OnSuccess_InvokesAction()
    {
        var tapped = 0;

        var returned = await SuccessAsync().TapAsync(v => tapped = v);

        Assert.Equal(42, tapped);
        Assert.True(returned.IsSuccess);
        Assert.Equal(42, returned.Value);
    }

    [Fact]
    public async Task TapAsync_Sync_OnFailure_DoesNotInvokeAction()
    {
        var tapped = 0;

        var returned = await FailureAsync(new BadRequestError("fail")).TapAsync(v => tapped = v);

        Assert.Equal(0, tapped);
        Assert.True(returned.IsFailure);
    }

    [Fact]
    public async Task TapAsync_Async_OnSuccess_InvokesAction()
    {
        var tapped = 0;

        var returned = await SuccessAsync().TapAsync(v =>
        {
            tapped = v;

            return Task.CompletedTask;
        });

        Assert.Equal(42, tapped);
        Assert.True(returned.IsSuccess);
        Assert.Equal(42, returned.Value);
    }

    [Fact]
    public async Task TapAsync_Async_OnFailure_DoesNotInvokeAction()
    {
        var tapped = 0;

        var returned = await FailureAsync(new BadRequestError("fail")).TapAsync(v =>
        {
            tapped = v;

            return Task.CompletedTask;
        });

        Assert.Equal(0, tapped);
        Assert.True(returned.IsFailure);
    }

    [Fact]
    public async Task BindAsync_ChainsValueProducingThenValuelessOperation()
    {
        // Mirrors a create-then-sign-in style chain off an async operation.
        var bound = await SuccessAsync()
            .BindAsync(v => Task.FromResult(Result<string>.Success(v.ToString())))
            .BindAsync(_ => Task.FromResult(Result.Success));

        Assert.True(bound.IsSuccess);
    }

    [Fact]
    public async Task BindAsync_ShortCircuitsChainOnFailure()
    {
        var error = new BadRequestError("fail");
        var secondInvoked = false;

        var bound = await FailureAsync(error)
            .BindAsync(_ =>
            {
                secondInvoked = true;

                return Task.FromResult(Result.Success);
            });

        Assert.False(secondInvoked);
        Assert.True(bound.IsFailure);
        Assert.Equal(error, bound.Error);
    }
}