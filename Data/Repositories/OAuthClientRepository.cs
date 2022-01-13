using System;
using System.Threading;
using System.Threading.Tasks;
using Data.Contracts;
using Entities.Entity;
using Microsoft.EntityFrameworkCore;

namespace Data.Repositories;

public class OAuthClientRepository : Repository<OAuthClient, int>, IOAuthClientRepository
{
    public OAuthClientRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<int?> GetOAuthClientIdByClientIdAndSecretCodeAsync(
        string clientId,
        Guid secretCode,
        CancellationToken cancellationToken)
    {
        OAuthClient oAuthClient = await TableNoTracking.SingleOrDefaultAsync(oc =>
                oc.Name == clientId &&
                oc.SecretCode == secretCode &&
                oc.Enabled,
            cancellationToken);

        return oAuthClient?.Id;
    }
}