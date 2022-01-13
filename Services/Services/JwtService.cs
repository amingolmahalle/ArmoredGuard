using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using Common.Extensions;
using Common.Helpers;
using Common.Settings;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Services.Contracts;
using Services.Dtos;

namespace Services.Services
{
    public class JwtService : IJwtService
    {
        private readonly SecuritySettings _securitySettings;

        public JwtService(IOptionsSnapshot<SecuritySettings> securitySettings)
        {
            _securitySettings = securitySettings.Value;
        }

        public AccessTokenDto GenerateToken(ClaimsDto claimsDto)
        {
            // longer that 16 character
            SecurityKey secretKey = Security.CreateSecurityKey(_securitySettings.JwtSettings.SecretKey);
            SigningCredentials signingCredentials = Security.CreateSigningCredentials(secretKey);

            // must be 16 character
            SecurityKey encryptionKey = Security.CreateEncryptionKey(_securitySettings.JwtSettings.EncryptKey);
            EncryptingCredentials encryptingCredentials = Security.CreateEncryptingCredentials(encryptionKey);

            IEnumerable<Claim> claims = _setClaims(claimsDto);

            SecurityTokenDescriptor descriptor =
                _createSecurityTokenDescriptor(signingCredentials, encryptingCredentials, claims);

            JwtSecurityToken jwtSecurityToken = new JwtSecurityTokenHandler().CreateJwtSecurityToken(descriptor);

            return new AccessTokenDto(jwtSecurityToken);
        }

        private SecurityTokenDescriptor _createSecurityTokenDescriptor(
            SigningCredentials signingCredentials,
            EncryptingCredentials encryptingCredentials,
            IEnumerable<Claim> claims)
        {
            SecurityTokenDescriptor descriptor = new SecurityTokenDescriptor
            {
                Issuer = _securitySettings.JwtSettings.Issuer,
                Audience = _securitySettings.JwtSettings.Audience,
                IssuedAt = DateTime.Now,
                NotBefore = DateTime.Now.AddMinutes(_securitySettings.JwtSettings.NotBeforeMinutes),
                Expires = DateTime.Now.AddMinutes(_securitySettings.JwtSettings.ExpirationMinutes),
                SigningCredentials = signingCredentials,
                EncryptingCredentials = encryptingCredentials,
                Subject = new ClaimsIdentity(claims)
            };
            return descriptor;
        }

        private IEnumerable<Claim> _setClaims(ClaimsDto claimsDto)
        {
            ICollection<Claim> claims = new Collection<Claim>();
            claims.AddUserId(claimsDto.UserId.ToString());
            claims.AddUserName(claimsDto.Username);
            claims.AddSecurityStamp(claimsDto.SecurityStamp);
            claims.AddFirstName(claimsDto.FirstName);
            claims.AddLastName(claimsDto.LastName);
            claims.AddMobileNumber(claimsDto.MobileNumber);
            claims.AddRoles(claimsDto.Roles.ToArray());

            return claims;
        }
    }
}