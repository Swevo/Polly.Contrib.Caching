// <copyright file="FakeCacheProvider.cs" company="Justin Bannister">
// Copyright (c) Justin Bannister. All rights reserved.
// </copyright>

using Polly.Contrib.Caching;

namespace Polly.Contrib.Caching.Tests;

internal sealed class FakeCacheProvider<TResult> : ICacheProvider<TResult>
{
    private readonly Dictionary<string, TResult?> _store = new();

    public int GetCount { get; private set; }
    public int SetCount { get; private set; }

    public bool TryGet(string key, out TResult? value)
    {
        GetCount++;
        return _store.TryGetValue(key, out value);
    }

    public void Set(string key, TResult? value, TimeSpan ttl)
    {
        SetCount++;
        _store[key] = value;
    }
}
