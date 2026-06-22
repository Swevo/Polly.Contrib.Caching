// <copyright file="ResiliencePipelineBuilderExtensionsTests.cs" company="Justin Bannister">
// Copyright (c) Justin Bannister. All rights reserved.
// </copyright>

using FluentAssertions;
using Microsoft.Extensions.Caching.Memory;
using Polly.Contrib.Caching;

namespace Polly.Contrib.Caching.Tests;

[TestFixture]
public class ResiliencePipelineBuilderExtensionsTests
{
    private IMemoryCache _cache = null!;

    [SetUp]
    public void SetUp() => _cache = new MemoryCache(new MemoryCacheOptions());

    [TearDown]
    public void TearDown() => _cache.Dispose();

    [Test]
    public void AddCaching_WithMemoryCache_BuildsWithoutThrowing()
    {
        var act = () => new ResiliencePipelineBuilder<string>()
            .AddCaching(_cache, _ => "key")
            .Build();

        act.Should().NotThrow();
    }

    [Test]
    public void AddCaching_WithOptions_BuildsWithoutThrowing()
    {
        var act = () => new ResiliencePipelineBuilder<string>()
            .AddCaching(new CachingStrategyOptions<string>
            {
                CacheProvider = new MemoryCacheProvider<string>(_cache),
                CacheKeyProvider = _ => "key",
            })
            .Build();

        act.Should().NotThrow();
    }

    [Test]
    public void AddCaching_NullBuilder_Throws()
    {
        ResiliencePipelineBuilder<string> builder = null!;

        var act = () => builder.AddCaching(_cache, _ => "key");

        act.Should().Throw<ArgumentNullException>().WithParameterName("builder");
    }

    [Test]
    public void AddCaching_NullCache_Throws()
    {
        var act = () => new ResiliencePipelineBuilder<string>()
            .AddCaching((IMemoryCache)null!, _ => "key");

        act.Should().Throw<ArgumentNullException>().WithParameterName("cache");
    }

    [Test]
    public void AddCaching_NullKeyProvider_Throws()
    {
        var act = () => new ResiliencePipelineBuilder<string>()
            .AddCaching(_cache, null!);

        act.Should().Throw<ArgumentNullException>().WithParameterName("cacheKeyProvider");
    }

    [Test]
    public void AddCaching_DefaultTtl_IsFiveMinutes()
    {
        var options = new CachingStrategyOptions<string>
        {
            CacheProvider = new MemoryCacheProvider<string>(_cache),
            CacheKeyProvider = _ => "key",
        };

        options.Ttl.Should().Be(TimeSpan.FromMinutes(5));
    }
}
