using System;

namespace Services.Dtos
{
    public class AddRefreshTokenDto
    {
        public Guid RefreshCode { get; set; }
        
        public int UserId { get; set; }
        
        public int OAuthClientId { get; set; }
        
        public DateTimeOffset CreatedAt { get; set; }
        
        public DateTimeOffset ExpireAt { get; set; }
    }
}