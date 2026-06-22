// <copyright file="ResiliencePipelineBuilderExtensions.cs" company="Justin Bannister">
// Copyright (c) Justin Bannister. All rights reserved.
// </copyright>

using Microsoft.Extensions.Caching.Memory;

namespace Polly.Contrib.Caching;

/// <summary>
/// Extension methods for adding caching to a <see cref="ResiliencePipelineBuilder{TResult}"/>.
/// </summary>
public static class ResiliencePipelineBuilderExtensions
{
    /// <summary>
    /// Adds a caching strategy using the supplied <see cref="CachingStrategyOptions{TResult}"/>.
    /// </summary>
    /// <typeparam name="TResult">The result type produced by the pipeline.</typeparam>
    /// <param name="builder">The pipeline builder.</param>
    /// <param name="options">Caching strategy configuration.</param>
    /// <returns>The same builder for chaining.</returns>
    public static ResiliencePipelineBuilder<TResult> AddCaching<TResult>(
        this ResiliencePipelineBuilder<TResult> builder,
        CachingStrategyOptions<TResult> options)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(options);

        return builder.AddStrategy(_ => new CachingResilienceStrategy<TResult>(options), options);
    }

    /// <summary>
    /// Adds a caching strategy backed by an <see cref="IMemoryCache"/>.
    /// </summary>
    /// <typeparam name="TResult">The result type produced by the pipeline.</typeparam>
    /// <param name="builder">The pipeline builder.</param>
    /// <param name="cache">The memory cache to use.</param>
    /// <param name="cacheKeyProvider">
    /// A delegate that returns a cache key from the current <see cref="ResilienceContext"/>.
    /// Return <c>null</c> to skip caching for a specific call.
    /// </param>
    /// <param name="ttl">How long cached entries live. Defaults to 5 minutes.</param>
    /// <returns>The same builder for chaining.</returns>
    public static ResiliencePipelineBuilder<TResult> AddCaching<TResult>(
        this ResiliencePipelineBuilder<TResult> builder,
        IMemoryCache cache,
        Func<ResilienceContext, string?> cacheKeyProvider,
        TimeSpan? ttl = null)
    {
        ArgumentNullException.ThrowIfNull(cache);
        ArgumentNullException.ThrowIfNull(cacheKeyProvider);

        return builder.AddCaching(new CachingStrategyOptions<TResult>
        {
            CacheProvider = new MemoryCacheProvider<TResult>(cache),
            CacheKeyProvider = cacheKeyProvider,
            Ttl = ttl ?? TimeSpan.FromMinutes(5),
        });
    }
}
