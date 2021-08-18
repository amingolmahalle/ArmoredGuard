using Services.Dtos;

namespace Services.Contracts
{
    public interface IJwtService
    {
        AccessTokenDto Generate(ClaimsDto claimsDto);
    }
}