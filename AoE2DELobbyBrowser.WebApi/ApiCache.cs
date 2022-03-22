using Microsoft.Extensions.Caching.Memory;

class ApiCache
{
    private static readonly SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1, 1);
    private readonly IMemoryCache _memoryCache;
    private readonly TimeSpan cacheExpiration;

    public ApiCache(IMemoryCache memoryCache, IConfiguration configuration)
    {
        _memoryCache = memoryCache;
        var exp = configuration.GetValue<int>("CacheExpiration");
        cacheExpiration = TimeSpan.FromSeconds(exp);
    }

    public async Task<string> GetOrCreateAsync(string key, Func<Task<String>> func)
    {
        if (!_memoryCache.TryGetValue<string>(key, out string data))
        {
            await semaphoreSlim.WaitAsync();
            try
            {
                if (!_memoryCache.TryGetValue<string>(key, out data))
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