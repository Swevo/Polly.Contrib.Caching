// <copyright file="CachingResilienceStrategyTests.cs" company="Justin Bannister">
// Copyright (c) Justin Bannister. All rights reserved.
// </copyright>

using FluentAssertions;
using Microsoft.Extensions.Caching.Memory;
using Polly.Contrib.Caching;

namespace Polly.Contrib.Caching.Tests;

[TestFixture]
public class CachingResilienceStrategyTests
{
    private IMemoryCache _cache = null!;
    private ResiliencePipeline<string> _pipeline = null!;
    private int _callCount;

    [SetUp]
    public void SetUp()
    {
        _cache = new MemoryCache(new MemoryCacheOptions());
        _callCount = 0;

        _pipeline = new ResiliencePipelineBuilder<string>()
            .AddCaching(_cache, _ => "test-key", ttl: TimeSpan.FromMinutes(5))
            .Build();
    }

    [TearDown]
    public void TearDown() => _cache.Dispose();

    [Test]
    public async Task CacheMiss_ExecutesCallbackAndReturnsResult()
    {
        var result = await _pipeline.ExecuteAsync(_ =>
        {
            _callCount++;
            return ValueTask.FromResult("hello");
        });

        result.Should().Be("hello");
        _callCount.Should().Be(1);
    }

    [Test]
    public async Task CacheHit_ReturnsWithoutInvokingCallback()
    {
        await _pipeline.ExecuteAsync(_ => ValueTask.FromResult("first"));

        var result = await _pipeline.ExecuteAsync(_ =>
        {
            _callCount++;
            return ValueTask.FromResult("second");
        });

        result.Should().Be("first");
        _callCount.Should().Be(0);
    }

    [Test]
    public async Task Exception_IsNotCached()
    {
        var shouldThrow = true;

        var result1 = await _pipeline.ExecuteOutcomeAsync<string, object>(async (_, _) =>
        {
            if (shouldThrow)
            {
                shouldThrow = false;
                return Outcome.FromException<string>(new InvalidOperationException("boom"));
            }
            return Outcome.FromResult("recovered");
        }, ResilienceContextPool.Shared.Get(), null!);

        result1.Exception.Should().NotBeNull();

        var result2 = await _pipeline.ExecuteAsync(_ => ValueTask.FromResult("recovered"));

        result2.Should().Be("recovered");
    }

    [Test]
    public async Task NullKeyFromProvider_SkipsCaching()
    {
        var pipeline = new ResiliencePipelineBuilder<string>()
            .AddCaching(_cache, _ => null)
            .Build();

        await pipeline.ExecuteAsync(_ => ValueTask.FromResult("first"));
        var result = await pipeline.ExecuteAsync(_ =>
        {
            _callCount++;
            return ValueTask.FromResult("second");
        });

        result.Should().Be("second");
        _callCount.Should().Be(1);
    }

    [Test]
    public async Task Ttl_ExpiresEntry()
    {
        var pipeline = new ResiliencePipelineBuilder<string>()
            .AddCaching(_cache, _ => "ttl-key", ttl: TimeSpan.FromMilliseconds(50))
            .Build();

        await pipeline.ExecuteAsync(_ => ValueTask.FromResult("original"));

        await Task.Delay(100);

        var result = await pipeline.ExecuteAsync(_ =>
        {
            _callCount++;
            return ValueTask.FromResult("refreshed");
        });

        result.Should().Be("refreshed");
        _callCount.Should().Be(1);
    }

    [Test]
    public async Task CustomCacheProvider_IsUsed()
    {
        var provider = new FakeCacheProvider<string>();

        var pipeline = new ResiliencePipelineBuilder<string>()
            .AddCaching(new CachingStrategyOptions<string>
            {
                CacheProvider = provider,
                CacheKeyProvider = _ => "key",
                Ttl = TimeSpan.FromMinutes(1),
            })
            .Build();

        await pipeline.ExecuteAsync(_ => ValueTask.FromResult("value"));
        await pipeline.ExecuteAsync(_ => ValueTask.FromResult("other"));

        provider.SetCount.Should().Be(1);
        provider.GetCount.Should().Be(2);
    }
}
