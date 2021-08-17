using System.Threading;
using System.Threading.Tasks;
using Entities.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Web.Controller.Base;
using Web.Models.RequestModels.Role;
using WebFramework.ApiResult;

namespace Web.Controller
{
    public class RolesController : BaseController
    {
        private readonly RoleManager<Role> _roleManager;

        public RolesController(RoleManager<Role> roleManager)
        {
            _roleManager = roleManager;
        }

        [HttpPost("create")]
        public async Task<ApiResult> Create(CreateRoleRequest request, CancellationToken cancellationToken)
        {
            await _roleManager.CreateAsync(new Role
            {
                Name = request.RoleName,
                Description = request.Description ?? request.RoleName.ToLower()
            });
            
            return Ok();
        }
    }
}