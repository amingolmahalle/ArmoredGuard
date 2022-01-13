using Services.Dtos;

namespace Services.Contracts;

public interface IJwtService
{
    AccessTokenDto GenerateToken(ClaimsDto claimsDto);
}