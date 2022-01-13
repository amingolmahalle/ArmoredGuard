using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Data.Contracts;
using Entities.Entity;
using Microsoft.EntityFrameworkCore;

namespace Data.Repositories;

public class OAuthRefreshTokenRepository : Repository<OAuthRefreshToken, int>, IOAuthRefreshTokenRepository
{
    public OAuthRefreshTokenRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }

    public Task<OAuthRefreshToken> GetOAuthRefreshTokenByUserIdAndRefreshCodeAndClientIdAsync(
        int userId,
        Guid refreshCode,
        int clientId,
        CancellationToken cancellationToken)
    {
        return TableNoTracking.SingleOrDefaultAsync(ort =>
                ort.CreatedBy == userId &&
                ort.RefreshCode == refreshCode &&
                ort.OAuthClientId == clientId,
            cancellationToken);
    }

    public async Task DeleteAllUserRefreshCodesAsync(
        int userId,
        CancellationToken cancellationToken)
    {
        var oAuthRefreshTokens = await Table
            .Where(ort => ort.CreatedBy == userId)
            .ToListAsync(cancellationToken: cancellationToken);

        await DeleteRangeAsync(oAuthRefreshTokens, cancellationToken);
    }
}