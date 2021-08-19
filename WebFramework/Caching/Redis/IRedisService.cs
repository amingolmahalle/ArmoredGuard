using System.Threading;
using System.Threading.Tasks;

namespace WebFramework.Caching.Redis
{
    public interface IRedisService
    {
        Task<T> GetAsync<T>(string key, CancellationToken cancellationToken) where T : class;
        
        Task SetAsync(string key, string value, short ttl, CancellationToken cancellationToken);

        Task RemoveAsync(string key, CancellationToken cancellationToken);
    }
}