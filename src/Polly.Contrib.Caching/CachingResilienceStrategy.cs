// <copyright file="CachingResilienceStrategy.cs" company="Justin Bannister">
// Copyright (c) Justin Bannister. All rights reserved.
// </copyright>

namespace Polly.Contrib.Caching;

/// <summary>
/// A Polly v8 resilience strategy that caches successful outcomes.
/// On a cache hit the downstream callback is not invoked.
/// On a cache miss the outcome is executed and — if successful — stored for subsequent calls.
/// </summary>
/// <typeparam name="TResult">The result type produced by the protected callback.</typeparam>
internal sealed class CachingResilienceStrategy<TResult>(CachingStrategyOptions<TResult> options)
    : ResilienceStrategy<TResult>
{
    /// <inheritdoc/>
    protected override async ValueTask<Outcome<TResult>> ExecuteCore<TState>(
        Func<ResilienceContext, TState, ValueTask<Outcome<TResult>>> callback,
        ResilienceContext context,
        TState state)
    {
        var key = options.CacheKeyProvider(context);

        if (key is not null && options.CacheProvider.TryGet(key, out var cached))
            return Outcome.FromResult(cached);

        var outcome = await callback(context, state).ConfigureAwait(false);

        if (key is not null && outcome.Exception is null)
            options.CacheProvider.Set(key, outcome.Result, options.Ttl);

        return outcome;
    }
}
