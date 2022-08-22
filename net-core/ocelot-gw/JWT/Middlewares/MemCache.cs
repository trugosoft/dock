using Microsoft.Extensions.Caching.Memory;

namespace JWT.Server.Middlewares.Cache
{
    public class MemCache
    {
        public MemoryCache Cache { get; set; }
        public MemCache()
        {
            Cache = new MemoryCache(new MemoryCacheOptions
            {
                SizeLimit = 4096,
                ExpirationScanFrequency = System.TimeSpan.FromMinutes(30)

            }); ;
        }
    }
}