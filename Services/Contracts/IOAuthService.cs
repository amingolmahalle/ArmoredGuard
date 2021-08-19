using System;
using System.Threading;
using System.Threading.Tasks;
using Entities.Entity;
using Services.Dtos;

namespace Services.Contracts
{
    public interface IOAuthService
    {
        Task<int?> GetOAuthClientIdByClientIdAndSecretCodeAsync(string clientId, Guid secretCode);

        Task RenewRefreshTokenAsync(RenewRefreshTokenDto request, CancellationToken cancellationToken);

        Task<OAuthRefreshToken> GetOAuthRefreshTokenByUserIdAndRefreshCodeAndClientIdAsync(
            int userId,
            Guid refreshCode,
            int clientId);

        Task AddRefreshTokenAsync(AddRefreshTokenDto request, CancellationToken cancellationToken);
    }
}