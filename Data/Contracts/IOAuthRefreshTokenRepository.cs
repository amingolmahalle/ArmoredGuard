using System;
using System.Threading;
using System.Threading.Tasks;
using Entities.Entity;

namespace Data.Contracts;

public interface IOAuthRefreshTokenRepository : IRepository<OAuthRefreshToken, int>
{
    Task<OAuthRefreshToken> GetOAuthRefreshTokenByUserIdAndRefreshCodeAndClientIdAsync(
        int userId,
        Guid refreshToken,
        int clientId,
        CancellationToken cancellationToken);

    Task DeleteAllUserRefreshCodesAsync(int userId, CancellationToken cancellationToken);
}