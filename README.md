# Polly.Contrib.Caching

<img src="icon.png" width="100" align="right" />

[![NuGet](https://img.shields.io/nuget/v/Polly.Contrib.Caching.svg)](https://www.nuget.org/packages/Polly.Contrib.Caching)
[![NuGet Downloads](https://img.shields.io/nuget/dt/Polly.Contrib.Caching.svg)](https://www.nuget.org/packages/Polly.Contrib.Caching)
[![CI](https://github.com/Swevo/Polly.Contrib.Caching/actions/workflows/build.yml/badge.svg)](https://github.com/Swevo/Polly.Contrib.Caching/actions/workflows/build.yml)

A caching resilience strategy for **Polly v8** pipelines. Restores the beloved Polly v7 caching policy for the new `DelayGenerator`-based API.

Polly v8 dropped the built-in caching policy. This package brings it back as a proper `ResilienceStrategy<T>` that plugs into any `ResiliencePipelineBuilder<TResult>`.

## Install

```
dotnet add package Polly.Contrib.Caching
```

## Usage

### With `IMemoryCache` (recommended)

```csharp
using Microsoft.Extensions.Caching.Memory;
using Polly.Contrib.Caching;

var cache = new MemoryCache(new MemoryCacheOptions());

var pipeline = new ResiliencePipelineBuilder<HttpResponseMessage>()
    .AddCaching(
        cache,
        cacheKeyProvider: ctx => ctx.Properties.TryGetValue(new ResiliencePropertyKey<string>("url"), out var url) ? url : null,
        ttl: TimeSpan.FromMinutes(5))
    .AddRetry(new RetryStrategyOptions<HttpResponseMessage>
    {
        MaxRetryAttempts = 3,
    })
    .Build();
```

### With `CachingStrategyOptions`

```csharp
var pipeline = new ResiliencePipelineBuilder<string>()
    .AddCaching(new CachingStrategyOptions<string>
    {
        CacheProvider = new MemoryCacheProvider<string>(cache),
        CacheKeyProvider = ctx => ctx.OperationKey,
        Ttl = TimeSpan.FromMinutes(10),
    })
    .Build();
```

### Custom `ICacheProvider`

Implement `ICacheProvider<TResult>` to use any cache store — distributed cache, Redis, etc.

```csharp
public sealed class RedisCacheProvider<TResult>(IConnectionMultiplexer redis) : ICacheProvider<TResult>
{
    public bool TryGet(string key, out TResult? value) { /* ... */ }
    public void Set(string key, TResult? value, TimeSpan ttl) { /* ... */ }
}
```

## Behaviour

| Scenario | Outcome |
|---|---|
| Cache hit | Returns cached result; downstream **not** called |
| Cache miss | Calls downstream; caches result on success |
| Downstream throws | Exception propagates; nothing cached |
| `CacheKeyProvider` returns `null` | Caching skipped; downstream always called |

## Composition with other strategies

Place `AddCaching` **before** retry/circuit-breaker so a cached result short-circuits the entire pipeline:

```csharp
var pipeline = new ResiliencePipelineBuilder<string>()
    .AddCaching(cache, ctx => ctx.OperationKey)   // 1. check cache first
    .AddRetry(...)                                  // 2. retry on failure
    .AddCircuitBreaker(...)                         // 3. protect downstream
    .Build();
```

## Support

If Polly.Contrib.Caching saves you time restoring the v7 caching policy, consider supporting the project:

[![Sponsor](https://img.shields.io/badge/Sponsor-%E2%9D%A4-pink?logo=github)](https://github.com/sponsors/Swevo)

> 💼 **Need .NET resilience help?** Visit [solidqualitysolutions.com](https://solidqualitysolutions.com/) for consulting and architecture services.

## Related packages

| Package | Description |
|---|---|
| [PollyChaos](https://www.nuget.org/packages/PollyChaos) | Chaos engineering — inject faults & latency (Simmy for v8) |
| [PollyMediatR](https://www.nuget.org/packages/PollyMediatR) | Polly v8 pipelines for MediatR request handlers |
| [PollyEFCore](https://www.nuget.org/packages/PollyEFCore) | Polly v8 resilience for EF Core queries and SaveChanges |
| [PollyBackoff](https://www.nuget.org/packages/PollyBackoff) | Backoff delay strategies |
| [PollyHealthChecks](https://www.nuget.org/packages/PollyHealthChecks) | [![Downloads](https://img.shields.io/nuget/dt/PollyHealthChecks.svg)](https://www.nuget.org/packages/PollyHealthChecks) | ASP.NET Core health checks for Polly v8 circuit breakers |
| [PollyOpenAI](https://www.nuget.org/packages/PollyOpenAI) | [![Downloads](https://img.shields.io/nuget/dt/PollyOpenAI.svg)](https://www.nuget.org/packages/PollyOpenAI) | Polly v8 resilience for OpenAI and Azure OpenAI — retry on 429, Retry-After, circuit breaker |
| [PollyRedis](https://www.nuget.org/packages/PollyRedis) | [![Downloads](https://img.shields.io/nuget/dt/PollyRedis.svg)](https://www.nuget.org/packages/PollyRedis) | Polly v8 resilience for StackExchange.Redis — retry, circuit breaker, timeout |
| [PollySignalR](https://www.nuget.org/packages/PollySignalR) | [![Downloads](https://img.shields.io/nuget/dt/PollySignalR.svg)](https://www.nuget.org/packages/PollySignalR) | Polly v8 exponential back-off reconnect policy for SignalR HubConnection |
| [PollyBulkhead](https://www.nuget.org/packages/PollyBulkhead) | Bulkhead isolation |
| [PollyRateLimiter](https://www.nuget.org/packages/PollyRateLimiter) | Rate limiting strategies |
| [PollyOpenTelemetry](https://www.nuget.org/packages/PollyOpenTelemetry) | OpenTelemetry metrics & tracing |

## License

MIT
