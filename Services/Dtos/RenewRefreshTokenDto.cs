using System;

namespace Services.Dtos;

public class RenewRefreshTokenDto
{
    public int UserId { get; set; }
        
    public int OAuthClientId { get; set; }
        
    public int OAuthRefreshTokenId { get; set; }
        
    public DateTimeOffset CreatedAt { get; set; }
        
    public DateTimeOffset ExpiresAt { get; set; }
        
    public Guid NewRefreshToken { get; set; }
}