using System;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace Common.Helpers
{
    public class SecurityHelper
    {
        public static string GetSha256Hash(string input)
        {
            using var sha256 = SHA256.Create();
            var byteValue = Encoding.UTF8.GetBytes(input);
            var byteHash = sha256.ComputeHash(byteValue);

            return Convert.ToBase64String(byteHash);
        }

        public static SecurityKey CreateSecurityKey(string securityKey)
        {
            return new SymmetricSecurityKey(Encoding.UTF8.GetBytes(securityKey));
        }

        public static SigningCredentials CreateSigningCredentials(SecurityKey securityKey)
        {
            return new(securityKey, SecurityAlgorithms.HmacSha256Signature);
        }

        public static SecurityKey CreateEncryptionKey(string securityKey)
        {
            return new SymmetricSecurityKey(Encoding.UTF8.GetBytes(securityKey));
        }

        public static EncryptingCredentials CreateEncryptingCredentials(SecurityKey encryptionKey)
        {
            return new(encryptionKey, SecurityAlgorithms.Aes128KW, SecurityAlgorithms.Aes128CbcHmacSha256);
        }
    }
}