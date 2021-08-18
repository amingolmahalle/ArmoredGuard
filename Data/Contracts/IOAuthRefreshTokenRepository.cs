using System;
using System.Threading.Tasks;
using Entities.OAuth;

namespace Data.Contracts
{
    public interface IOAuthRefreshTokenRepository : IRepository<OAuthRefreshToken>
    {
        Task<OAuthRefreshToken> GetOAuthRefreshTokenUserIdAndRefreshCodeAndClientIdAsync(
            int userId,
            Guid refreshToken,
            int clientId);
    }
}