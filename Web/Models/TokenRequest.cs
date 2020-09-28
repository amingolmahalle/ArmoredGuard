using System.ComponentModel.DataAnnotations;

namespace Web.Models
{
    public class TokenRequest
    {
        [Required]
        public string GrantType { get; set; }
        
        public string Username { get; set; }
        
        public string Password { get; set; }
        
        public string RefreshToken { get; set; }
        
        public string Scope { get; set; }

        public string ClientId { get; set; }
        
        public string ClientSecret { get; set; }
    }
}
