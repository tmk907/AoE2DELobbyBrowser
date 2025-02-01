using Microsoft.Extensions.Caching.Memory;

public class ApiCache
{
    public const string LobbiesKey = "lobbies";

    private static readonly SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1, 1);
    private readonly IMemoryCache _memoryCache;
    private readonly TimeSpan cacheExpiration;

    public ApiCache(IMemoryCache memoryCache, IConfiguration configuration)
    {
        _memoryCache = memoryCache;
        var exp = configuration.GetValue<int>("CacheExpiration");
        cacheExpiration = TimeSpan.FromSeconds(exp);
    }

    public T? Get<T>(string key)
    {
        var data = _memoryCache.Get<T>(key);
        return data;
    }

    public void Set<T>(string key, T data)
    {
        _memoryCache.Set(key, data, cacheExpiration);
    }

    public async Task<T?> GetOrCreateAsync<T>(string key, Func<Task<T>> func)
    {
        if (!_memoryCache.TryGetValue<T>(key, out T? data))
        {
            await semaphoreSlim.WaitAsync();
            try
            {
                if (!_memoryCache.TryGetValue<T>(key, out data))
                {
                    data = await func();
                    _memoryCache.Set(key, data, cacheExpiration);
                }
            }
            finally
            {
                semaphoreSlim.Release();
            }
        }
        return data;
    }
}