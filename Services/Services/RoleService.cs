using System.Threading.Tasks;
using Entities.Entity;
using Microsoft.AspNetCore.Identity;
using Services.Contracts;

namespace Services.Services
{
    public class RoleService : IRoleService
    {
        private readonly RoleManager<Role> _roleManager;

        public RoleService(RoleManager<Role> roleManager)
        {
            _roleManager = roleManager;
        }

        public async Task CreateAsync(Role role)
        {
            await _roleManager.CreateAsync(role);
        }

        public Task<Role> FindByIdAsync(string roleId)
        {
            return _roleManager.FindByIdAsync(roleId);
        }
    }
}