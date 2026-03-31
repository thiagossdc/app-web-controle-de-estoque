using System.Collections.Concurrent;
using Microsoft.Extensions.Caching.Memory;

namespace Estoque.Api.Services;

public interface IPerformanceOptimizationService
{
    Task<T> GetOrSetAsync<T>(string key, Func<Task<T>> getItem, TimeSpan? expiration = null);
    Task InvalidateCacheAsync(string pattern);
    Task PreloadCacheAsync();
    Task<PerformanceMetrics> GetPerformanceMetricsAsync();
}

public class PerformanceOptimizationService : IPerformanceOptimizationService
{
    private readonly IMemoryCache _cache;
    private readonly ILogger<PerformanceOptimizationService> _logger;
    private readonly ConcurrentDictionary<string, DateTime> _cacheTimestamps = new();
    private readonly ConcurrentDictionary<string, int> _cacheAccessCount = new();
    private readonly ConcurrentDictionary<string, double> _queryDurations = new();
    private readonly DateTime _startTime = DateTime.UtcNow;

    public PerformanceOptimizationService(IMemoryCache cache, ILogger<PerformanceOptimizationService> logger)
    {
        _cache = cache;
        _logger = logger;
    }

    public async Task<T> GetOrSetAsync<T>(string key, Func<Task<T>> getItem, TimeSpan? expiration = null)
    {
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        
        if (_cache.TryGetValue(key, out T? cachedValue) && cachedValue is not null)
        {
            stopwatch.Stop();
            RecordCacheHit(key, stopwatch.ElapsedMilliseconds);
            return cachedValue;
        }

        stopwatch.Restart();
        var item = await getItem();
        stopwatch.Stop();
        
        RecordCacheMiss(key, stopwatch.ElapsedMilliseconds);
        
        var options = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = expiration ?? TimeSpan.FromMinutes(30),
            SlidingExpiration = TimeSpan.FromMinutes(10),
            Priority = CacheItemPriority.Normal
        };

        options.RegisterPostEvictionCallback((evictedKey, evictedValue, reason, state) =>
        {
            _cacheTimestamps.TryRemove(evictedKey.ToString()!, out _);
            _cacheAccessCount.TryRemove(evictedKey.ToString()!, out _);
            _logger.LogDebug("Cache evicted: {Key}, Reason: {Reason}", evictedKey, reason);
        });

        _cache.Set(key, item, options);
        _cacheTimestamps[key] = DateTime.UtcNow;
        _cacheAccessCount[key] = 0;
        
        return item;
    }

    public Task InvalidateCacheAsync(string pattern)
    {
        var keysToRemove = _cacheTimestamps.Keys
            .Where(k => k.StartsWith(pattern, StringComparison.OrdinalIgnoreCase))
            .ToList();

        foreach (var key in keysToRemove)
        {
            _cache.Remove(key);
            _cacheTimestamps.TryRemove(key, out _);
            _cacheAccessCount.TryRemove(key, out _);
        }

        _logger.LogInformation("Cache invalidated for pattern: {Pattern}, Keys removed: {Count}", 
            pattern, keysToRemove.Count);
        
        return Task.CompletedTask;
    }

    public async Task PreloadCacheAsync()
    {
        _logger.LogInformation("Starting cache preload...");
        
        // Pré-carregar dados comuns
        var preloadTasks = new List<Task>
        {
            PreloadCategoriesAsync(),
            PreloadSuppliersAsync(),
            PreloadDashboardDataAsync()
        };

        await Task.WhenAll(preloadTasks);
        _logger.LogInformation("Cache preload completed");
    }

    public Task<PerformanceMetrics> GetPerformanceMetricsAsync()
    {
        var uptime = DateTime.UtcNow - _startTime;
        var totalCacheAccess = _cacheAccessCount.Values.Sum();
        var avgQueryDuration = _queryDurations.Values.Any() ? _queryDurations.Values.Average() : 0;
        
        var metrics = new PerformanceMetrics
        {
            Uptime = uptime,
            TotalCacheEntries = _cacheTimestamps.Count,
            TotalCacheAccess = totalCacheAccess,
            AverageQueryDurationMs = avgQueryDuration,
            CacheHitRate = CalculateCacheHitRate(),
            MemoryUsageBytes = GC.GetTotalMemory(false),
            ActiveConnections = GetActiveConnections()
        };

        return Task.FromResult(metrics);
    }

    private void RecordCacheHit(string key, long durationMs)
    {
        _cacheAccessCount.AddOrUpdate(key, 1, (k, v) => v + 1);
        _queryDurations.AddOrUpdate($"hit_{key}", durationMs, (k, v) => (v + durationMs) / 2);
        
        _logger.LogDebug("Cache hit: {Key}, Duration: {Duration}ms", key, durationMs);
    }

    private void RecordCacheMiss(string key, long durationMs)
    {
        _queryDurations.AddOrUpdate($"miss_{key}", durationMs, (k, v) => (v + durationMs) / 2);
        
        _logger.LogDebug("Cache miss: {Key}, Duration: {Duration}ms", key, durationMs);
    }

    private double CalculateCacheHitRate()
    {
        var totalHits = _cacheAccessCount.Values.Sum();
        var totalMisses = _queryDurations.Keys.Count(k => k.StartsWith("miss_"));
        var totalRequests = totalHits + totalMisses;
        
        return totalRequests > 0 ? (double)totalHits / totalRequests * 100 : 0;
    }

    private int GetActiveConnections()
    {
        // Normalmente viria do pool de conexões ou serviço de métricas
        return Environment.ProcessorCount * 2;
    }

    private async Task PreloadCategoriesAsync()
    {
        // Implementação depende da camada de acesso a dados
        await Task.Delay(100); // Placeholder
    }

    private async Task PreloadSuppliersAsync()
    {
        // Implementação depende da camada de acesso a dados
        await Task.Delay(100); // Placeholder
    }

    private async Task PreloadDashboardDataAsync()
    {
        // Implementação depende da camada de acesso a dados
        await Task.Delay(100); // Placeholder
    }
}

public class PerformanceMetrics
{
    public TimeSpan Uptime { get; set; }
    public int TotalCacheEntries { get; set; }
    public int TotalCacheAccess { get; set; }
    public double AverageQueryDurationMs { get; set; }
    public double CacheHitRate { get; set; }
    public long MemoryUsageBytes { get; set; }
    public int ActiveConnections { get; set; }
}

