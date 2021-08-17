using System.Threading.Tasks;
using Services.DomainModels;
using Services.Dtos;

namespace Services.Services
{
    public interface IJwtService
    {
        AccessTokenDto Generate(ClaimsDto claimsDto);
    }
}