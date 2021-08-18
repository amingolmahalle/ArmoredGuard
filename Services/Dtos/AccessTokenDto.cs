using System;
using System.IdentityModel.Tokens.Jwt;

namespace Services.Dtos
{
    public class AccessTokenDto
    {
        private readonly DateTime _dateTimeNow = DateTime.UtcNow;

        public AccessTokenDto(JwtSecurityToken securityToken)
        {
            access_token = new JwtSecurityTokenHandler().WriteToken(securityToken);
            token_type = "Bearer";
            refresh_token = Guid.NewGuid().ToString();
            expires_in = (int) (securityToken.ValidTo - _dateTimeNow).TotalSeconds;
            CreatedAt = _dateTimeNow;
            ExpiresAt = securityToken.ValidTo;
        }

        public string access_token { get; set; }

        public string refresh_token { get; set; }

        public string token_type { get; set; }

        public int expires_in { get; set; }

        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset ExpiresAt { get; set; }
    }
}