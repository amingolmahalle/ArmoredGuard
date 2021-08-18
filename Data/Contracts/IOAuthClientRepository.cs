using System;
using System.Threading.Tasks;
using Entities.OAuth;

namespace Data.Contracts
{
    public interface IOAuthClientRepository : IRepository<OAuthClient>
    {
        Task<bool> IsExistOAuthClientByClientIdAndSecretCodeAsync(string clientId, Guid secretCode);
        
        Task<int?> GetOAuthClientIdByClientIdAndSecretCodeAsync(string clientId, Guid secretCode);
    }
}