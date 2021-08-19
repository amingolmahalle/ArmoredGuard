using System.Threading;
using System.Threading.Tasks;

namespace Services.Contracts.Redis
{
    public interface IRedisService
    {
        Task<T> GetAsync<T>(string key, CancellationToken cancellationToken) where T : class;

        Task SetAsync(string key, string value, short ttl, CancellationToken cancellationToken);

        Task<bool> IsExistAsync(string key, CancellationToken cancellationToken);

        Task RemoveAsync(string key, CancellationToken cancellationToken);
    }
}