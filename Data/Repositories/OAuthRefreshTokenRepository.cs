using System;
using System.Threading.Tasks;
using Data.Contracts;
using Entities.OAuth;
using Microsoft.EntityFrameworkCore;

namespace Data.Repositories
{
    public class OAuthRefreshTokenRepository : Repository<OAuthRefreshToken>, IOAuthRefreshTokenRepository
    {
        public OAuthRefreshTokenRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public Task<OAuthRefreshToken> GetOAuthRefreshTokenUserIdAndRefreshCodeAndClientIdAsync(
            int userId,
            Guid refreshCode,
            int clientId)
        {
            return TableNoTracking.SingleOrDefaultAsync(ort =>
                ort.CreatedBy == userId &&
                ort.RefreshCode == refreshCode &&
                ort.OAuthClientId == clientId);
        }
    }
}