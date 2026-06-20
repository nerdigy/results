namespace Nerdigy.Results.Tests;

public sealed class ResultTTests
{
    [Fact]
    public void Success_IsSuccess_ReturnsTrue()
    {
        var result = Result<int>.Success(42);

        Assert.True(result.IsSuccess);
        Assert.False(result.IsFailure);
        Assert.Equal(42, result.Value);
    }

    [Fact]
    public void Failure_IsFailure_ReturnsTrue()
    {
        var result = Result<int>.Failure(new BadRequestError("fail"));

        Assert.True(result.IsFailure);
        Assert.False(result.IsSuccess);
    }

    [Fact]
    public void Failure_Value_ThrowsInvalidOperationException()
    {
        var result = Result<int>.Failure(new BadRequestError("fail"));

        Assert.Throws<InvalidOperationException>(() => result.Value);
    }

    [Fact]
    public void Success_Error_ThrowsInvalidOperationException()
    {
        var result = Result<int>.Success(42);

        Assert.Throws<InvalidOperationException>(() => result.Error);
    }

    [Fact]
    public void ImplicitConversion_FromValue_CreatesSuccess()
    {
        Result<int> result = 42;

        Assert.True(result.IsSuccess);
        Assert.Equal(42, result.Value);
    }

    [Fact]
    public void ImplicitConversion_FromError_CreatesFailure()
    {
        Result<int> result = new BadRequestError("fail");

        Assert.True(result.IsFailure);
    }

    [Fact]
    public void Match_OnSuccess_InvokesOnSuccess()
    {
        Result<int> result = 42;

        var value = result.Match(v => v.ToString(), _ => "failure");

        Assert.Equal("42", value);
    }

    [Fact]
    public void Match_OnFailure_InvokesOnFailure()
    {
        Result<int> result = new BadRequestError("fail");

        var value = result.Match(v => v.ToString(), e => ((BadRequestError)e).Message);

        Assert.Equal("fail", value);
    }

    [Fact]
    public void Map_OnSuccess_TransformsValue()
    {
        Result<int> result = 42;

        var mapped = result.Map(v => v.ToString());

        Assert.True(mapped.IsSuccess);
        Assert.Equal("42", mapped.Value);
    }

    [Fact]
    public void Map_OnFailure_PreservesError()
    {
        var error = new BadRequestError("fail");
        Result<int> result = error;

        var mapped = result.Map(v => v.ToString());

        Assert.True(mapped.IsFailure);
        Assert.Equal(error, mapped.Error);
    }

    [Fact]
    public void Bind_OnSuccess_ReturnsChainedResult()
    {
        Result<int> result = 42;

        var bound = result.Bind(v => Result<string>.Success(v.ToString()));

        Assert.True(bound.IsSuccess);
        Assert.Equal("42", bound.Value);
    }

    [Fact]
    public void Bind_OnFailure_ReturnsFailureWithOriginalError()
    {
        var error = new BadRequestError("fail");
        Result<int> result = error;

        var bound = result.Bind(v => Result<string>.Success(v.ToString()));

        Assert.True(bound.IsFailure);
        Assert.Equal(error, bound.Error);
    }

    [Fact]
    public void BindToResult_OnSuccess_ReturnsChainedResult()
    {
        Result<int> result = 42;

        var bound = result.Bind(_ => Result.Success);

        Assert.True(bound.IsSuccess);
    }

    [Fact]
    public void BindToResult_OnFailure_ReturnsFailureWithOriginalError()
    {
        var error = new BadRequestError("fail");
        Result<int> result = error;

        var bound = result.Bind(_ => Result.Success);

        Assert.True(bound.IsFailure);
        Assert.Equal(error, bound.Error);
    }

    [Fact]
    public void Tap_OnSuccess_InvokesAction()
    {
        Result<int> result = 42;
        var tappedValue = 0;

        var returned = result.Tap(v => tappedValue = v);

        Assert.Equal(42, tappedValue);
        Assert.True(returned.IsSuccess);
        Assert.Equal(42, returned.Value);
    }

    [Fact]
    public void Tap_OnFailure_DoesNotInvokeAction()
    {
        Result<int> result = new BadRequestError("fail");
        var tapped = false;

        var returned = result.Tap(_ => tapped = true);

        Assert.False(tapped);
        Assert.True(returned.IsFailure);
    }

    [Fact]
    public async Task MatchAsync_OnSuccess_InvokesOnSuccess()
    {
        Result<int> result = 42;

        var value = await result.MatchAsync(
            v => Task.FromResult(v.ToString()),
            _ => Task.FromResult("failure"));

        Assert.Equal("42", value);
    }

    [Fact]
    public async Task MatchAsync_OnFailure_InvokesOnFailure()
    {
        Result<int> result = new BadRequestError("fail");

        var value = await result.MatchAsync(
            v => Task.FromResult(v.ToString()),
            e => Task.FromResult(((BadRequestError)e).Message));

        Assert.Equal("fail", value);
    }

    [Fact]
    public async Task MapAsync_OnSuccess_TransformsValue()
    {
        Result<int> result = 42;

        var mapped = await result.MapAsync(v => Task.FromResult(v.ToString()));

        Assert.True(mapped.IsSuccess);
        Assert.Equal("42", mapped.Value);
    }

    [Fact]
    public async Task MapAsync_OnFailure_PreservesError()
    {
        var error = new BadRequestError("fail");
        Result<int> result = error;

        var mapped = await result.MapAsync(v => Task.FromResult(v.ToString()));

        Assert.True(mapped.IsFailure);
        Assert.Equal(error, mapped.Error);
    }

    [Fact]
    public async Task BindAsync_OnSuccess_ReturnsChainedResult()
    {
        Result<int> result = 42;

        var bound = await result.BindAsync(v => Task.FromResult(Result<string>.Success(v.ToString())));

        Assert.True(bound.IsSuccess);
        Assert.Equal("42", bound.Value);
    }

    [Fact]
    public async Task BindAsync_OnFailure_ReturnsFailureWithOriginalError()
    {
        var error = new BadRequestError("fail");
        Result<int> result = error;

        var bound = await result.BindAsync(v => Task.FromResult(Result<string>.Success(v.ToString())));

        Assert.True(bound.IsFailure);
        Assert.Equal(error, bound.Error);
    }

    [Fact]
    public async Task BindAsyncToResult_OnSuccess_ReturnsChainedResult()
    {
        Result<int> result = 42;

        var bound = await result.BindAsync(_ => Task.FromResult(Result.Success));

        Assert.True(bound.IsSuccess);
    }

    [Fact]
    public async Task BindAsyncToResult_OnFailure_ReturnsFailureWithOriginalError()
    {
        var error = new BadRequestError("fail");
        Result<int> result = error;

        var bound = await result.BindAsync(_ => Task.FromResult(Result.Success));

        Assert.True(bound.IsFailure);
        Assert.Equal(error, bound.Error);
    }

    [Fact]
    public async Task TapAsync_OnSuccess_InvokesAction()
    {
        Result<int> result = 42;
        var tappedValue = 0;

        var returned = await result.TapAsync(v =>
        {
            tappedValue = v;

            return Task.CompletedTask;
        });

        Assert.Equal(42, tappedValue);
        Assert.True(returned.IsSuccess);
    }

    [Fact]
    public async Task TapAsync_OnFailure_DoesNotInvokeAction()
    {
        Result<int> result = new BadRequestError("fail");
        var tapped = false;

        var returned = await result.TapAsync(_ =>
        {
            tapped = true;

            return Task.CompletedTask;
        });

        Assert.False(tapped);
        Assert.True(returned.IsFailure);
    }

    [Fact]
    public void TapError_OnFailure_InvokesAction()
    {
        var error = new BadRequestError("fail");
        Result<int> result = error;
        Error? tappedError = null;

        var returned = result.TapError(e => tappedError = e);

        Assert.Equal(error, tappedError);
        Assert.True(returned.IsFailure);
        Assert.Equal(error, returned.Error);
    }

    [Fact]
    public void TapError_OnSuccess_DoesNotInvokeAction()
    {
        Result<int> result = 42;
        var tapped = false;

        var returned = result.TapError(_ => tapped = true);

        Assert.False(tapped);
        Assert.True(returned.IsSuccess);
        Assert.Equal(42, returned.Value);
    }

    [Fact]
    public async Task TapErrorAsync_OnFailure_InvokesAction()
    {
        var error = new BadRequestError("fail");
        Result<int> result = error;
        Error? tappedError = null;

        var returned = await result.TapErrorAsync(e =>
        {
            tappedError = e;

            return Task.CompletedTask;
        });

        Assert.Equal(error, tappedError);
        Assert.True(returned.IsFailure);
        Assert.Equal(error, returned.Error);
    }

    [Fact]
    public async Task TapErrorAsync_OnSuccess_DoesNotInvokeAction()
    {
        Result<int> result = 42;
        var tapped = false;

        var returned = await result.TapErrorAsync(_ =>
        {
            tapped = true;

            return Task.CompletedTask;
        });

        Assert.False(tapped);
        Assert.True(returned.IsSuccess);
        Assert.Equal(42, returned.Value);
    }
}