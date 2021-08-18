using System;
using System.Threading;
using System.Threading.Tasks;
using Data.Contracts;
using Entities.OAuth;
using Services.Dtos;

namespace Services.Services
{
    public class OAuthService : IOAuthService
    {
        private readonly IOAuthClientRepository _oAuthClientRepository;

        private readonly IOAuthRefreshTokenRepository _oAuthRefreshTokenRepository;

        public OAuthService(
            IOAuthClientRepository oAuthClientRepository,
            IOAuthRefreshTokenRepository oAuthRefreshTokenRepository)
        {
            _oAuthClientRepository = oAuthClientRepository;
            _oAuthRefreshTokenRepository = oAuthRefreshTokenRepository;
        }

        public Task<bool> IsApplicantValidAsync(string clientId, Guid secretCode)
        {
            return _oAuthClientRepository.IsExistOAuthClientByClientIdAndSecretCodeAsync(clientId, secretCode);
        }

        public Task<int?> GetOAuthClientIdByClientIdAndSecretCodeAsync(
            string clientId, Guid secretCode)
        {
            return _oAuthClientRepository.GetOAuthClientIdByClientIdAndSecretCodeAsync(clientId, secretCode);
        }

        public Task<OAuthRefreshToken> GetOAuthRefreshTokenByUserIdAndRefreshCodeAndClientIdAsync(
            int userId,
            Guid refreshToken,
            int clientId)
        {
            return _oAuthRefreshTokenRepository.GetOAuthRefreshTokenUserIdAndRefreshCodeAndClientIdAsync(
                userId,
                refreshToken,
                clientId);


            //TODO: return dto
        }

        public async Task RenewRefreshTokenAsync(RenewRefreshTokenDto request, CancellationToken cancellationToken)
        {
            OAuthRefreshToken oAuthRefreshToken = new OAuthRefreshToken {Id = request.OAuthRefreshTokenId};

            await _oAuthRefreshTokenRepository.DeleteAsync(oAuthRefreshToken, cancellationToken);

            var newOAuthRefreshToken = new OAuthRefreshToken
            {
                CreatedBy = request.UserId,
                OAuthClientId =request.OAuthClientId,
                RefreshCode = request.NewRefreshToken,
                CreatedAt = request.CreatedAt,
                ExpiresAt = request.ExpiresAt
            };

            await _oAuthRefreshTokenRepository.AddAsync(newOAuthRefreshToken, cancellationToken);
        }
    }
}