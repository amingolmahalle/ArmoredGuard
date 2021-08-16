using System.Threading.Tasks;
using Services.DomainModels;

namespace Services.Services
{
    public interface IJwtService
    {
        AccessToken Generate(ClaimsDto claimsDto);
    }
}