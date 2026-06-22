// <copyright file="CachingStrategyOptions.cs" company="Justin Bannister">
// Copyright (c) Justin Bannister. All rights reserved.
// </copyright>

namespace Polly.Contrib.Caching;

/// <summary>
/// Options for configuring the <see cref="CachingResilienceStrategy{TResult}"/>.
/// </summary>
/// <typeparam name="TResult">The result type to cache.</typeparam>
public sealed class CachingStrategyOptions<TResult> : ResilienceStrategyOptions
{
    /// <summary>
    /// Initializes a new instance of <see cref="CachingStrategyOptions{TResult}"/>.
    /// </summary>
    public CachingStrategyOptions() => Name = "Caching";

    /// <summary>
    /// The cache provider to use for storing and retrieving results.
    /// </summary>
    public required ICacheProvider<TResult> CacheProvider { get; set; }

    /// <summary>
    /// A delegate that returns a cache key from the current <see cref="ResilienceContext"/>.
    /// Return <c>null</c> to skip caching for a specific call.
    /// </summary>
    public required Func<ResilienceContext, string?> CacheKeyProvider { get; set; }

    /// <summary>
    /// How long cached entries live. Defaults to 5 minutes.
    /// </summary>
    public TimeSpan Ttl { get; set; } = TimeSpan.FromMinutes(5);
}
