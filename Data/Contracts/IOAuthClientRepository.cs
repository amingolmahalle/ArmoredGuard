using System;
using System.Threading;
using System.Threading.Tasks;
using Entities.Entity;

namespace Data.Contracts
{
    public interface IOAuthClientRepository : IRepository<OAuthClient, int>
    {
        Task<int?> GetOAuthClientIdByClientIdAndSecretCodeAsync(
            string clientId,
            Guid secretCode,
            CancellationToken cancellationToken);
    }
}