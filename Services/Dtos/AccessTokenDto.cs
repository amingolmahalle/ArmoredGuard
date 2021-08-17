using System;
using System.IdentityModel.Tokens.Jwt;
using System.Text.Json;

namespace Services.Dtos
{
    public class AccessTokenDto
    {
        public AccessTokenDto(JwtSecurityToken securityToken)
        {
            access_token = new JwtSecurityTokenHandler().WriteToken(securityToken);
            token_type = "Bearer";
            expires_in = (int) (securityToken.ValidTo - DateTime.UtcNow).TotalSeconds;
        }

        public string access_token { get; set; }

        public string refresh_token { get; set; }

        public string token_type { get; set; }

        public int expires_in { get; set; }
    }
}