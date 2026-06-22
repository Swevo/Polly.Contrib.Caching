// <copyright file="MemoryCacheProvider.cs" company="Justin Bannister">
// Copyright (c) Justin Bannister. All rights reserved.
// </copyright>

using Microsoft.Extensions.Caching.Memory;

namespace Polly.Contrib.Caching;

/// <summary>
/// An <see cref="ICacheProvider{TResult}"/> backed by <see cref="IMemoryCache"/>.
/// </summary>
/// <typeparam name="TResult">The type of result being cached.</typeparam>
public sealed class MemoryCacheProvider<TResult>(IMemoryCache cache) : ICacheProvider<TResult>
{
    /// <inheritdoc/>
    public bool TryGet(string key, out TResult? value) => cache.TryGetValue(key, out value);

    /// <inheritdoc/>
    public void Set(string key, TResult? value, TimeSpan ttl) => cache.Set(key, value, ttl);
}
