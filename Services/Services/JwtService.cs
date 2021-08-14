using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Common.Settings;
using Entities.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Services.DomainModels;

namespace Services.Services
{
    public class JwtService : IJwtService
    {
        private readonly SecuritySettings _securitySettings;

        private readonly SignInManager<User> _signInManager;

        public JwtService(IOptionsSnapshot<SecuritySettings> securitySettings, SignInManager<User> signInManager)
        {
            _securitySettings = securitySettings.Value;
            _signInManager = signInManager;
        }

        public async Task<AccessToken> GenerateAsync(ClaimsDto claimsDto)
        {
            var secretKey = Encoding.UTF8.GetBytes(_securitySettings.JwtSettings.SecretKey); // longer that 16 character
            var signingCredentials = new SigningCredentials(new SymmetricSecurityKey(secretKey),
                SecurityAlgorithms.HmacSha256Signature);

            var encryptionKey = Encoding.UTF8.GetBytes(_securitySettings.JwtSettings.EncryptKey); //must be 16 character
            var encryptingCredentials = new EncryptingCredentials(
                new SymmetricSecurityKey(encryptionKey),
                SecurityAlgorithms.Aes128KW,
                SecurityAlgorithms.Aes128CbcHmacSha256);

            var claims = await _setClaimsAsync(claimsDto);

            var descriptor = new SecurityTokenDescriptor
            {
                Issuer = _securitySettings.JwtSettings.Issuer,
                Audience = _securitySettings.JwtSettings.Audience,
                IssuedAt = DateTime.Now,
                NotBefore = DateTime.Now.AddMinutes(_securitySettings.JwtSettings.NotBeforeMinutes),
                Expires = DateTime.Now.AddHours(_securitySettings.JwtSettings.ExpirationTime),
                SigningCredentials = signingCredentials,
                EncryptingCredentials = encryptingCredentials,
                Subject = new ClaimsIdentity(claims)
            };

            //JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            //JwtSecurityTokenHandler.DefaultMapInboundClaims = false;
            //JwtSecurityTokenHandler.DefaultOutboundClaimTypeMap.Clear();

            var tokenHandler = new JwtSecurityTokenHandler();

            var securityToken = tokenHandler.CreateJwtSecurityToken(descriptor);

            //string encryptedJwt = tokenHandler.WriteToken(securityToken);

            return new AccessToken(securityToken);
        }

        private async Task<IEnumerable<Claim>> _setClaimsAsync(ClaimsDto claimsDto)
        {
            // ClaimsPrincipal result = await _signInManager.ClaimsFactory.CreateAsync(user);

            //add custom claims
            return new List<Claim>
            {
                new(ClaimTypes.Role, claimsDto.RoleName),
                new(ClaimTypes.NameIdentifier, claimsDto.UserId.ToString()),
                new(ClaimTypes.Name, claimsDto.Username),
                new(new ClaimsIdentityOptions().SecurityStampClaimType, claimsDto.SecurityStampClaim)
            };
        }
    }
}