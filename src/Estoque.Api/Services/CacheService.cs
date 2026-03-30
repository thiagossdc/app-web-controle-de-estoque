using Microsoft.Extensions.Caching.Memory;

namespace Estoque.Api.Services;

public interface ICacheService
{
    Task<T?> GetAsync<T>(string key);
    Task SetAsync<T>(string key, T value, TimeSpan? expiration = null);
    Task RemoveAsync(string key);
    Task RemoveByPrefixAsync(string prefix);
}

public class MemoryCacheService : ICacheService
{
    private readonly IMemoryCache _cache;
    private readonly ILogger<MemoryCacheService> _logger;
    private readonly HashSet<string> _cacheKeys = new();

    public MemoryCacheService(IMemoryCache cache, ILogger<MemoryCacheService> logger)
    {
        _cache = cache;
        _logger = logger;
    }

    public Task<T?> GetAsync<T>(string key)
    {
        _logger.LogDebug("Buscando cache para chave: {Key}", key);
        var value = _cache.Get<T>(key);
        return Task.FromResult(value);
    }

    public Task SetAsync<T>(string key, T value, TimeSpan? expiration = null)
    {
        var options = new MemoryCacheEntryOptions();
        
        if (expiration.HasValue)
        {
            options.AbsoluteExpirationRelativeToNow = expiration.Value;
        }
        else
        {
            options.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30);
        }

        options.RegisterPostEvictionCallback((evictedKey, evictedValue, reason, state) =>
        {
            _cacheKeys.Remove(evictedKey.ToString()!);
            _logger.LogDebug("Cache removido: {Key}, Razão: {Reason}", evictedKey, reason);
        });

        _cache.Set(key, value, options);
        _cacheKeys.Add(key);
        
        _logger.LogDebug("Cache definido para chave: {Key}, Expiração: {Expiration}", key, expiration);
        return Task.CompletedTask;
    }

    public Task RemoveAsync(string key)
    {
        _cache.Remove(key);
        _cacheKeys.Remove(key);
        _logger.LogDebug("Cache removido para chave: {Key}", key);
        return Task.CompletedTask;
    }

    public Task RemoveByPrefixAsync(string prefix)
    {
        var keysToRemove = _cacheKeys.Where(k => k.StartsWith(prefix)).ToList();
        
        foreach (var key in keysToRemove)
        {
            _cache.Remove(key);
            _cacheKeys.Remove(key);
        }
        
        _logger.LogDebug("Cache removido para prefixo: {Prefix}, Chaves removidas: {Count}", prefix, keysToRemove.Count);
        return Task.CompletedTask;
    }
}

public static class CacheKeys
{
    public const string Products = "products";
    public const string Categories = "categories";
    public const string Suppliers = "suppliers";
    public const string Dashboard = "dashboard";
    
    public static string ProductById(int id) => $"product:{id}";
    public static string CategoryById(int id) => $"category:{id}";
    public static string SupplierById(int id) => $"supplier:{id}";
    public static string ProductsByCategory(int categoryId) => $"products:category:{categoryId}";
    public static string ProductsBySupplier(int supplierId) => $"products:supplier:{supplierId}";
}