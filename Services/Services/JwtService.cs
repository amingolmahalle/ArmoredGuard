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
using Services.DomainModels;

namespace Services.Services
{
    public class JwtService : IJwtService
    {
        private readonly SecuritySettings _securitySettings;

        // private readonly SignInManager<User> _signInManager;

        public JwtService(IOptionsSnapshot<SecuritySettings> securitySettings)
        {
            _securitySettings = securitySettings.Value;
        }

        public AccessToken Generate(ClaimsDto claimsDto)
        {
            // longer that 16 character
            SecurityKey secretKey = SecurityHelper.CreateSecurityKey(_securitySettings.JwtSettings.SecretKey);
            SigningCredentials signingCredentials = SecurityHelper.CreateSigningCredentials(secretKey);

            // must be 16 character
            SecurityKey encryptionKey = SecurityHelper.CreateEncryptionKey(_securitySettings.JwtSettings.EncryptKey);
            EncryptingCredentials encryptingCredentials = SecurityHelper.CreateEncryptingCredentials(encryptionKey);

            IEnumerable<Claim> claims = _setClaims(claimsDto);

            SecurityTokenDescriptor descriptor =
                _createSecurityTokenDescriptor(signingCredentials, encryptingCredentials, claims);

            //JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            //JwtSecurityTokenHandler.DefaultMapInboundClaims = false;
            //JwtSecurityTokenHandler.DefaultOutboundClaimTypeMap.Clear();

            JwtSecurityToken jwtSecurityToken = new JwtSecurityTokenHandler().CreateJwtSecurityToken(descriptor);

            return new AccessToken(jwtSecurityToken);
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
                Expires = DateTime.Now.AddHours(_securitySettings.JwtSettings.ExpirationTime),
                SigningCredentials = signingCredentials,
                EncryptingCredentials = encryptingCredentials,
                Subject = new ClaimsIdentity(claims)
            };
            return descriptor;
        }

        private IEnumerable<Claim> _setClaims(ClaimsDto claimsDto)
        {
            // ClaimsPrincipal result = await _signInManager.ClaimsFactory.CreateAsync(user);

            //add custom claims
            ICollection<Claim> claims = new Collection<Claim>();
            claims.AddUserId(claimsDto.UserId.ToString());
            claims.AddUserName(claimsDto.Username);
            claims.AddSecurityStamp(claimsDto.SecurityStamp);
            claims.AddRoles(claimsDto.RolesName.ToArray());

            return claims;
        }
    }
}