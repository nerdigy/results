namespace Nerdigy.Results;

/// <summary>
/// Provides extension methods for <see cref="Result"/>.
/// </summary>
public static class ResultExtensions
{
    extension(Result result)
    {
        /// <summary>
        /// Matches the result to one of two functions based on success or failure.
        /// </summary>
        /// <typeparam name="TResult">The type of the return value.</typeparam>
        /// <param name="onSuccess">The function to invoke when the result is successful.</param>
        /// <param name="onFailure">The function to invoke when the result is a failure.</param>
        /// <returns>The value produced by the matched function.</returns>
        public TResult Match<TResult>(Func<TResult> onSuccess, Func<Error, TResult> onFailure)
        {
            return result.IsSuccess ? onSuccess() : onFailure(result.Error);
        }

        /// <summary>
        /// Chains to another <see cref="Result"/> producing operation on success.
        /// </summary>
        /// <param name="onSuccess">The function to invoke when the result is successful.</param>
        /// <returns>The result of <paramref name="onSuccess"/> if successful; otherwise, the original failure.</returns>
        public Result Bind(Func<Result> onSuccess)
        {
            return result.IsSuccess ? onSuccess() : result;
        }

        /// <summary>
        /// Chains to a <see cref="Result{TValue}"/> producing operation on success.
        /// </summary>
        /// <typeparam name="TValue">The type of the value in the resulting result.</typeparam>
        /// <param name="onSuccess">The function to invoke when the result is successful.</param>
        /// <returns>The result of <paramref name="onSuccess"/> if successful; otherwise, a failed result with the original error.</returns>
        public Result<TValue> Bind<TValue>(Func<Result<TValue>> onSuccess)
        {
            return result.IsSuccess ? onSuccess() : Result<TValue>.Failure(result.Error);
        }

        /// <summary>
        /// Runs a side-effect action on success without changing the result.
        /// </summary>
        /// <param name="onSuccess">The action to invoke when the result is successful.</param>
        /// <returns>The original result.</returns>
        public Result Tap(Action onSuccess)
        {
            if (result.IsSuccess)
            {
                onSuccess();
            }

            return result;
        }

        /// <summary>
        /// Asynchronously matches the result to one of two functions based on success or failure.
        /// </summary>
        /// <typeparam name="TResult">The type of the return value.</typeparam>
        /// <param name="onSuccess">The async function to invoke when the result is successful.</param>
        /// <param name="onFailure">The async function to invoke when the result is a failure.</param>
        /// <returns>The value produced by the matched function.</returns>
        public async Task<TResult> MatchAsync<TResult>(Func<Task<TResult>> onSuccess, Func<Error, Task<TResult>> onFailure)
        {
            return result.IsSuccess ? await onSuccess() : await onFailure(result.Error);
        }

        /// <summary>
        /// Asynchronously chains to another <see cref="Result"/> producing operation on success.
        /// </summary>
        /// <param name="onSuccess">The async function to invoke when the result is successful.</param>
        /// <returns>The result of <paramref name="onSuccess"/> if successful; otherwise, the original failure.</returns>
        public async Task<Result> BindAsync(Func<Task<Result>> onSuccess)
        {
            return result.IsSuccess ? await onSuccess() : result;
        }

        /// <summary>
        /// Asynchronously chains to a <see cref="Result{TValue}"/> producing operation on success.
        /// </summary>
        /// <typeparam name="TValue">The type of the value in the resulting result.</typeparam>
        /// <param name="onSuccess">The async function to invoke when the result is successful.</param>
        /// <returns>The result of <paramref name="onSuccess"/> if successful; otherwise, a failed result with the original error.</returns>
        public async Task<Result<TValue>> BindAsync<TValue>(Func<Task<Result<TValue>>> onSuccess)
        {
            return result.IsSuccess ? await onSuccess() : Result<TValue>.Failure(result.Error);
        }

        /// <summary>
        /// Asynchronously runs a side-effect on success without changing the result.
        /// </summary>
        /// <param name="onSuccess">The async action to invoke when the result is successful.</param>
        /// <returns>The original result.</returns>
        public async Task<Result> TapAsync(Func<Task> onSuccess)
        {
            if (result.IsSuccess)
            {
                await onSuccess();
            }

            return result;
        }
    }

    extension(Task<Result> task)
    {
        /// <summary>
        /// Awaits the result, then matches it to one of two functions based on success or failure.
        /// </summary>
        /// <typeparam name="TResult">The type of the return value.</typeparam>
        /// <param name="onSuccess">The function to invoke when the result is successful.</param>
        /// <param name="onFailure">The function to invoke when the result is a failure.</param>
        /// <returns>The value produced by the matched function.</returns>
        public async Task<TResult> MatchAsync<TResult>(Func<TResult> onSuccess, Func<Error, TResult> onFailure)
        {
            var result = await task;
            return result.IsSuccess ? onSuccess() : onFailure(result.Error);
        }

        /// <summary>
        /// Awaits the result, then asynchronously matches it to one of two functions based on success or failure.
        /// </summary>
        /// <typeparam name="TResult">The type of the return value.</typeparam>
        /// <param name="onSuccess">The async function to invoke when the result is successful.</param>
        /// <param name="onFailure">The async function to invoke when the result is a failure.</param>
        /// <returns>The value produced by the matched function.</returns>
        public async Task<TResult> MatchAsync<TResult>(Func<Task<TResult>> onSuccess, Func<Error, Task<TResult>> onFailure)
        {
            var result = await task;
            return result.IsSuccess ? await onSuccess() : await onFailure(result.Error);
        }

        /// <summary>
        /// Awaits the result, then chains to another <see cref="Result"/> producing operation on success.
        /// </summary>
        /// <param name="onSuccess">The function to invoke when the result is successful.</param>
        /// <returns>The result of <paramref name="onSuccess"/> if successful; otherwise, the original failure.</returns>
        public async Task<Result> BindAsync(Func<Result> onSuccess)
        {
            var result = await task;
            return result.IsSuccess ? onSuccess() : result;
        }

        /// <summary>
        /// Awaits the result, then asynchronously chains to another <see cref="Result"/> producing operation on success.
        /// </summary>
        /// <param name="onSuccess">The async function to invoke when the result is successful.</param>
        /// <returns>The result of <paramref name="onSuccess"/> if successful; otherwise, the original failure.</returns>
        public async Task<Result> BindAsync(Func<Task<Result>> onSuccess)
        {
            var result = await task;
            return result.IsSuccess ? await onSuccess() : result;
        }

        /// <summary>
        /// Awaits the result, then chains to a <see cref="Result{TValue}"/> producing operation on success.
        /// </summary>
        /// <typeparam name="TValue">The type of the value in the resulting result.</typeparam>
        /// <param name="onSuccess">The function to invoke when the result is successful.</param>
        /// <returns>The result of <paramref name="onSuccess"/> if successful; otherwise, a failed result with the original error.</returns>
        public async Task<Result<TValue>> BindAsync<TValue>(Func<Result<TValue>> onSuccess)
        {
            var result = await task;
            return result.IsSuccess ? onSuccess() : Result<TValue>.Failure(result.Error);
        }

        /// <summary>
        /// Awaits the result, then asynchronously chains to a <see cref="Result{TValue}"/> producing operation on success.
        /// </summary>
        /// <typeparam name="TValue">The type of the value in the resulting result.</typeparam>
        /// <param name="onSuccess">The async function to invoke when the result is successful.</param>
        /// <returns>The result of <paramref name="onSuccess"/> if successful; otherwise, a failed result with the original error.</returns>
        public async Task<Result<TValue>> BindAsync<TValue>(Func<Task<Result<TValue>>> onSuccess)
        {
            var result = await task;
            return result.IsSuccess ? await onSuccess() : Result<TValue>.Failure(result.Error);
        }

        /// <summary>
        /// Awaits the result, then runs a side-effect action on success without changing the result.
        /// </summary>
        /// <param name="onSuccess">The action to invoke when the result is successful.</param>
        /// <returns>The original result.</returns>
        public async Task<Result> TapAsync(Action onSuccess)
        {
            var result = await task;

            if (result.IsSuccess)
            {
                onSuccess();
            }

            return result;
        }

        /// <summary>
        /// Awaits the result, then asynchronously runs a side-effect on success without changing the result.
        /// </summary>
        /// <param name="onSuccess">The async action to invoke when the result is successful.</param>
        /// <returns>The original result.</returns>
        public async Task<Result> TapAsync(Func<Task> onSuccess)
        {
            var result = await task;

            if (result.IsSuccess)
            {
                await onSuccess();
            }

            return result;
        }
    }
}

/// <summary>
/// Provides extension methods for <see cref="Result{TValue}"/>.
/// </summary>
public static class ResultTExtensions
{
    extension<TValue>(Result<TValue> result)
    {
        /// <summary>
        /// Matches the result to one of two functions based on success or failure.
        /// </summary>
        /// <typeparam name="TResult">The type of the return value.</typeparam>
        /// <param name="onSuccess">The function to invoke with the value when the result is successful.</param>
        /// <param name="onFailure">The function to invoke with the error when the result is a failure.</param>
        /// <returns>The value produced by the matched function.</returns>
        public TResult Match<TResult>(Func<TValue, TResult> onSuccess, Func<Error, TResult> onFailure)
        {
            return result.IsSuccess ? onSuccess(result.Value) : onFailure(result.Error);
        }

        /// <summary>
        /// Transforms the success value using the specified mapping function.
        /// </summary>
        /// <typeparam name="TResult">The type of the transformed value.</typeparam>
        /// <param name="mapper">The function to transform the success value.</param>
        /// <returns>A new result with the transformed value if successful; otherwise, a failed result with the original error.</returns>
        public Result<TResult> Map<TResult>(Func<TValue, TResult> mapper)
        {
            return result.IsSuccess ? Result<TResult>.Success(mapper(result.Value)) : Result<TResult>.Failure(result.Error);
        }

        /// <summary>
        /// Chains to another <see cref="Result{TResult}"/> producing operation on success.
        /// </summary>
        /// <typeparam name="TResult">The type of the value in the resulting result.</typeparam>
        /// <param name="onSuccess">The function to invoke with the value when the result is successful.</param>
        /// <returns>The result of <paramref name="onSuccess"/> if successful; otherwise, a failed result with the original error.</returns>
        public Result<TResult> Bind<TResult>(Func<TValue, Result<TResult>> onSuccess)
        {
            return result.IsSuccess ? onSuccess(result.Value) : Result<TResult>.Failure(result.Error);
        }

        /// <summary>
        /// Chains to a <see cref="Result"/> producing operation on success.
        /// </summary>
        /// <param name="onSuccess">The function to invoke with the value when the result is successful.</param>
        /// <returns>The result of <paramref name="onSuccess"/> if successful; otherwise, a failed result with the original error.</returns>
        public Result Bind(Func<TValue, Result> onSuccess)
        {
            return result.IsSuccess ? onSuccess(result.Value) : Result.Failure(result.Error);
        }

        /// <summary>
        /// Runs a side-effect action on success without changing the result.
        /// </summary>
        /// <param name="onSuccess">The action to invoke with the value when the result is successful.</param>
        /// <returns>The original result.</returns>
        public Result<TValue> Tap(Action<TValue> onSuccess)
        {
            if (result.IsSuccess)
            {
                onSuccess(result.Value);
            }

            return result;
        }

        /// <summary>
        /// Asynchronously matches the result to one of two functions based on success or failure.
        /// </summary>
        /// <typeparam name="TResult">The type of the return value.</typeparam>
        /// <param name="onSuccess">The async function to invoke with the value when the result is successful.</param>
        /// <param name="onFailure">The async function to invoke with the error when the result is a failure.</param>
        /// <returns>The value produced by the matched function.</returns>
        public async Task<TResult> MatchAsync<TResult>(Func<TValue, Task<TResult>> onSuccess, Func<Error, Task<TResult>> onFailure)
        {
            return result.IsSuccess ? await onSuccess(result.Value) : await onFailure(result.Error);
        }

        /// <summary>
        /// Asynchronously transforms the success value using the specified mapping function.
        /// </summary>
        /// <typeparam name="TResult">The type of the transformed value.</typeparam>
        /// <param name="mapper">The async function to transform the success value.</param>
        /// <returns>A new result with the transformed value if successful; otherwise, a failed result with the original error.</returns>
        public async Task<Result<TResult>> MapAsync<TResult>(Func<TValue, Task<TResult>> mapper)
        {
            return result.IsSuccess ? Result<TResult>.Success(await mapper(result.Value)) : Result<TResult>.Failure(result.Error);
        }

        /// <summary>
        /// Asynchronously chains to another <see cref="Result{TResult}"/> producing operation on success.
        /// </summary>
        /// <typeparam name="TResult">The type of the value in the resulting result.</typeparam>
        /// <param name="onSuccess">The async function to invoke with the value when the result is successful.</param>
        /// <returns>The result of <paramref name="onSuccess"/> if successful; otherwise, a failed result with the original error.</returns>
        public async Task<Result<TResult>> BindAsync<TResult>(Func<TValue, Task<Result<TResult>>> onSuccess)
        {
            return result.IsSuccess ? await onSuccess(result.Value) : Result<TResult>.Failure(result.Error);
        }

        /// <summary>
        /// Asynchronously chains to a <see cref="Result"/> producing operation on success.
        /// </summary>
        /// <param name="onSuccess">The async function to invoke with the value when the result is successful.</param>
        /// <returns>The result of <paramref name="onSuccess"/> if successful; otherwise, a failed result with the original error.</returns>
        public async Task<Result> BindAsync(Func<TValue, Task<Result>> onSuccess)
        {
            return result.IsSuccess ? await onSuccess(result.Value) : Result.Failure(result.Error);
        }

        /// <summary>
        /// Asynchronously runs a side-effect on success without changing the result.
        /// </summary>
        /// <param name="onSuccess">The async action to invoke with the value when the result is successful.</param>
        /// <returns>The original result.</returns>
        public async Task<Result<TValue>> TapAsync(Func<TValue, Task> onSuccess)
        {
            if (result.IsSuccess)
            {
                await onSuccess(result.Value);
            }

            return result;
        }
    }

    extension<TValue>(Task<Result<TValue>> task)
    {
        /// <summary>
        /// Awaits the result, then matches it to one of two functions based on success or failure.
        /// </summary>
        /// <typeparam name="TResult">The type of the return value.</typeparam>
        /// <param name="onSuccess">The function to invoke with the value when the result is successful.</param>
        /// <param name="onFailure">The function to invoke with the error when the result is a failure.</param>
        /// <returns>The value produced by the matched function.</returns>
        public async Task<TResult> MatchAsync<TResult>(Func<TValue, TResult> onSuccess, Func<Error, TResult> onFailure)
        {
            var result = await task;
            return result.IsSuccess ? onSuccess(result.Value) : onFailure(result.Error);
        }

        /// <summary>
        /// Awaits the result, then asynchronously matches it to one of two functions based on success or failure.
        /// </summary>
        /// <typeparam name="TResult">The type of the return value.</typeparam>
        /// <param name="onSuccess">The async function to invoke with the value when the result is successful.</param>
        /// <param name="onFailure">The async function to invoke with the error when the result is a failure.</param>
        /// <returns>The value produced by the matched function.</returns>
        public async Task<TResult> MatchAsync<TResult>(Func<TValue, Task<TResult>> onSuccess, Func<Error, Task<TResult>> onFailure)
        {
            var result = await task;
            return result.IsSuccess ? await onSuccess(result.Value) : await onFailure(result.Error);
        }

        /// <summary>
        /// Awaits the result, then transforms the success value using the specified mapping function.
        /// </summary>
        /// <typeparam name="TResult">The type of the transformed value.</typeparam>
        /// <param name="mapper">The function to transform the success value.</param>
        /// <returns>A new result with the transformed value if successful; otherwise, a failed result with the original error.</returns>
        public async Task<Result<TResult>> MapAsync<TResult>(Func<TValue, TResult> mapper)
        {
            var result = await task;
            return result.IsSuccess ? Result<TResult>.Success(mapper(result.Value)) : Result<TResult>.Failure(result.Error);
        }

        /// <summary>
        /// Awaits the result, then asynchronously transforms the success value using the specified mapping function.
        /// </summary>
        /// <typeparam name="TResult">The type of the transformed value.</typeparam>
        /// <param name="mapper">The async function to transform the success value.</param>
        /// <returns>A new result with the transformed value if successful; otherwise, a failed result with the original error.</returns>
        public async Task<Result<TResult>> MapAsync<TResult>(Func<TValue, Task<TResult>> mapper)
        {
            var result = await task;
            return result.IsSuccess ? Result<TResult>.Success(await mapper(result.Value)) : Result<TResult>.Failure(result.Error);
        }

        /// <summary>
        /// Awaits the result, then chains to another <see cref="Result{TResult}"/> producing operation on success.
        /// </summary>
        /// <typeparam name="TResult">The type of the value in the resulting result.</typeparam>
        /// <param name="onSuccess">The function to invoke with the value when the result is successful.</param>
        /// <returns>The result of <paramref name="onSuccess"/> if successful; otherwise, a failed result with the original error.</returns>
        public async Task<Result<TResult>> BindAsync<TResult>(Func<TValue, Result<TResult>> onSuccess)
        {
            var result = await task;
            return result.IsSuccess ? onSuccess(result.Value) : Result<TResult>.Failure(result.Error);
        }

        /// <summary>
        /// Awaits the result, then asynchronously chains to another <see cref="Result{TResult}"/> producing operation on success.
        /// </summary>
        /// <typeparam name="TResult">The type of the value in the resulting result.</typeparam>
        /// <param name="onSuccess">The async function to invoke with the value when the result is successful.</param>
        /// <returns>The result of <paramref name="onSuccess"/> if successful; otherwise, a failed result with the original error.</returns>
        public async Task<Result<TResult>> BindAsync<TResult>(Func<TValue, Task<Result<TResult>>> onSuccess)
        {
            var result = await task;
            return result.IsSuccess ? await onSuccess(result.Value) : Result<TResult>.Failure(result.Error);
        }

        /// <summary>
        /// Awaits the result, then chains to a <see cref="Result"/> producing operation on success.
        /// </summary>
        /// <param name="onSuccess">The function to invoke with the value when the result is successful.</param>
        /// <returns>The result of <paramref name="onSuccess"/> if successful; otherwise, a failed result with the original error.</returns>
        public async Task<Result> BindAsync(Func<TValue, Result> onSuccess)
        {
            var result = await task;
            return result.IsSuccess ? onSuccess(result.Value) : Result.Failure(result.Error);
        }

        /// <summary>
        /// Awaits the result, then asynchronously chains to a <see cref="Result"/> producing operation on success.
        /// </summary>
        /// <param name="onSuccess">The async function to invoke with the value when the result is successful.</param>
        /// <returns>The result of <paramref name="onSuccess"/> if successful; otherwise, a failed result with the original error.</returns>
        public async Task<Result> BindAsync(Func<TValue, Task<Result>> onSuccess)
        {
            var result = await task;
            return result.IsSuccess ? await onSuccess(result.Value) : Result.Failure(result.Error);
        }

        /// <summary>
        /// Awaits the result, then runs a side-effect action on success without changing the result.
        /// </summary>
        /// <param name="onSuccess">The action to invoke with the value when the result is successful.</param>
        /// <returns>The original result.</returns>
        public async Task<Result<TValue>> TapAsync(Action<TValue> onSuccess)
        {
            var result = await task;

            if (result.IsSuccess)
            {
                onSuccess(result.Value);
            }

            return result;
        }

        /// <summary>
        /// Awaits the result, then asynchronously runs a side-effect on success without changing the result.
        /// </summary>
        /// <param name="onSuccess">The async action to invoke with the value when the result is successful.</param>
        /// <returns>The original result.</returns>
        public async Task<Result<TValue>> TapAsync(Func<TValue, Task> onSuccess)
        {
            var result = await task;

            if (result.IsSuccess)
            {
                await onSuccess(result.Value);
            }

            return result;
        }
    }
}