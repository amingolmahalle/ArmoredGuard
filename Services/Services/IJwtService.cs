using System.Threading.Tasks;
using Services.DomainModels;

namespace Services.Services
{
    public interface IJwtService
    {
        Task<AccessToken> GenerateAsync(ClaimsDto claimsDto);
    }
}