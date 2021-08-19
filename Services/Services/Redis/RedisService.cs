using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace Services.Services.Redis
{
    public class RedisService : IRedisService
    {
        private readonly IDistributedCache _distributedCache;

        public RedisService(IDistributedCache distributedCache)
        {
            _distributedCache = distributedCache;
        }

        public async Task<T> GetAsync<T>(string key, CancellationToken cancellationToken) where T : class
        {
            T value = default;

            if (string.IsNullOrEmpty(key))
                throw new Exception("key should not be empty");

            Byte[] encodedValue = await _distributedCache.GetAsync(key, cancellationToken);

            if (encodedValue != null)
            {
                string serializedValue = Encoding.UTF8.GetString(encodedValue);

                value = JsonConvert.DeserializeObject<T>(serializedValue);
            }

            return value;
        }

        public async Task SetAsync(string key, string value, short ttl, CancellationToken cancellationToken)
        {
            Byte[] encodedValue = Encoding.UTF8.GetBytes(value);

            var options = new DistributedCacheEntryOptions().SetAbsoluteExpiration(DateTime.Now.AddMinutes(ttl));

            await _distributedCache.SetAsync(key, encodedValue, options, cancellationToken);
        }

        public async Task<bool> IsExistAsync(string key, CancellationToken cancellationToken)
        {
            bool hasValue = false;
            Byte[] value = await _distributedCache.GetAsync(key, cancellationToken);

            if (value != null && value.Length > 0)
                hasValue = true;

            return hasValue;
        }

        public async Task RemoveAsync(string key, CancellationToken cancellationToken)
        {
            await _distributedCache.RemoveAsync(key, cancellationToken);
        }
    }
}