using Microsoft.Extensions.Caching.Memory;

namespace OnlineClassifieds.Services
{
    public class CacheService<T>
    {
        private readonly IMemoryCache _cache;
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

        public CacheService(IMemoryCache cache)
        {
            _cache = cache;
        }

        public async Task Create(object key, Func<Task<T>> createItem)
        {
            await _semaphore.WaitAsync();  // запрашиваем разрешение
            try
            {
                T cacheEntry = await createItem();

                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromMinutes(5))  // интервал времени, в течение которого запись будет оставаться в кэше
                    .SetAbsoluteExpiration(TimeSpan.FromMinutes(20));  // макс. время, в течение которого запись может оставаться в кэше

                _cache.Set(key, cacheEntry, cacheEntryOptions);
            }
            finally
            {
                _semaphore.Release();  // освобождаем разрешение
            }
        }

        public void SetItem(object key, T item)
        {
            _cache.Set(key, item);
        }

        public async Task<T> GetOrCreate(object key, Func<Task<T>> createItem)
        {
            T cacheEntry;

            if (!_cache.TryGetValue(key, out cacheEntry))
            {
                await _semaphore.WaitAsync();  // запрашиваем разрешение
                try
                {
                    if (!_cache.TryGetValue(key, out cacheEntry))
                    {
                        cacheEntry = await createItem();

                        var cacheEntryOptions = new MemoryCacheEntryOptions()
                            .SetSlidingExpiration(TimeSpan.FromMinutes(5))  // интервал времени, в течение которого запись будет оставаться в кэше
                            .SetAbsoluteExpiration(TimeSpan.FromMinutes(20));  // макс. время, в течение которого запись может оставаться в кэше

                        _cache.Set(key, cacheEntry, cacheEntryOptions);
                    }
                }
                finally
                {
                    _semaphore.Release();  // освобождаем разрешение
                }
            }
            return cacheEntry;
        }
    }
}
