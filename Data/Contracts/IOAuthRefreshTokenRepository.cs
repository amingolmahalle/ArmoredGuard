using System;
using System.Threading.Tasks;
using Entities.Entity;

namespace Data.Contracts
{
    public interface IOAuthRefreshTokenRepository : IRepository<OAuthRefreshToken,int>
    {
        Task<OAuthRefreshToken> GetOAuthRefreshTokenUserIdAndRefreshCodeAndClientIdAsync(
            int userId,
            Guid refreshToken,
            int clientId);
    }
}