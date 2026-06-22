// <copyright file="ICacheProvider.cs" company="Justin Bannister">
// Copyright (c) Justin Bannister. All rights reserved.
// </copyright>

namespace Polly.Contrib.Caching;

/// <summary>
/// Abstracts a cache store for use with <see cref="CachingStrategyOptions{TResult}"/>.
/// </summary>
/// <typeparam name="TResult">The type of result being cached.</typeparam>
public interface ICacheProvider<TResult>
{
    /// <summary>
    /// Attempts to retrieve a cached value.
    /// </summary>
    /// <param name="key">The cache key.</param>
    /// <param name="value">The cached value if found.</param>
    /// <returns><c>true</c> if a value was found; otherwise <c>false</c>.</returns>
    bool TryGet(string key, out TResult? value);

    /// <summary>
    /// Stores a value in the cache with the specified TTL.
    /// </summary>
    /// <param name="key">The cache key.</param>
    /// <param name="value">The value to cache.</param>
    /// <param name="ttl">How long the entry should live.</param>
    void Set(string key, TResult? value, TimeSpan ttl);
}
