using System;
using System.Threading.Tasks;
using Data.Contracts;
using Entities.OAuth;
using Microsoft.EntityFrameworkCore;

namespace Data.Repositories
{
    public class OAuthClientRepository : Repository<OAuthClient>, IOAuthClientRepository
    {
        public OAuthClientRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public Task<bool> IsExistOAuthClientByClientIdAndSecretCodeAsync(string clientId, Guid secretCode)
        {
            return TableNoTracking.AnyAsync(oc =>
                oc.Name == clientId &&
                oc.SecretCode == secretCode &&
                oc.Enabled);
        }

        public async Task<int?> GetOAuthClientIdByClientIdAndSecretCodeAsync(string clientId, Guid secretCode)
        {
            OAuthClient oAuthClient = await TableNoTracking.SingleOrDefaultAsync(oc =>
                oc.Name == clientId &&
                oc.SecretCode == secretCode &&
                oc.Enabled);

            return oAuthClient?.Id;
        }
    }
}