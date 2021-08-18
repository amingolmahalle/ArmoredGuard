using System;
using System.Threading;
using System.Threading.Tasks;
using Entities.OAuth;
using Services.Dtos;

namespace Services.Services
{
    public interface IOAuthService
    {
        Task<bool> IsApplicantValidAsync(string clientId, Guid secretCode);

        Task<int?> GetOAuthClientIdByClientIdAndSecretCodeAsync(string clientId, Guid secretCode);

        Task RenewRefreshTokenAsync(RenewRefreshTokenDto request, CancellationToken cancellationToken);

        Task<OAuthRefreshToken> GetOAuthRefreshTokenByUserIdAndRefreshCodeAndClientIdAsync(int userId, Guid refreshCode, int clientId);
    }
}