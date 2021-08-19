using System;
using System.IdentityModel.Tokens.Jwt;
using Newtonsoft.Json;

namespace Services.Dtos
{
    public class AccessTokenDto
    {
        private readonly DateTime _dateTimeNow = DateTime.UtcNow;

        public AccessTokenDto(JwtSecurityToken securityToken)
        {
            AccessToken = new JwtSecurityTokenHandler().WriteToken(securityToken);
            TokenType = "Bearer";
            RefreshToken = Guid.NewGuid().ToString();
            ExpiresIn = (int) (securityToken.ValidTo - _dateTimeNow).TotalSeconds;
            CreatedAt = _dateTimeNow;
            ExpiresAt = securityToken.ValidTo;
        }

        [JsonProperty("access_token")] public string AccessToken { get; set; }

        [JsonProperty("refresh_token")] public string RefreshToken { get; set; }

        [JsonProperty("token_type")] public string TokenType { get; set; }

        [JsonProperty("expires_in")] public int ExpiresIn { get; set; }

        [JsonProperty("Created_at")] public DateTimeOffset CreatedAt { get; set; }

        [JsonProperty("Expires_at")] public DateTimeOffset ExpiresAt { get; set; }
    }
}