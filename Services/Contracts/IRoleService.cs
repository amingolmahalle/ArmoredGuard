using System.Threading.Tasks;
using Entities.Entity;

namespace Services.Contracts
{
    public interface IRoleService
    {
        Task CreateAsync(Role role);

        Task<Role> FindByIdAsync(string roleId);
    }
}