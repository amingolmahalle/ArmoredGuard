using System.Threading;
using System.Threading.Tasks;
using Entities.Entity;
using Microsoft.AspNetCore.Mvc;
using Services.Contracts;
using Web.Controller.Base;
using Web.Models.RequestModels.Role;

namespace Web.Controller
{
    public class RolesController : BaseController
    {
        private readonly IRoleService _roleService;

        public RolesController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        [HttpPost("create")]
        public async Task<ApiResult.ApiResult> Create(CreateRoleRequest request, CancellationToken cancellationToken)
        {
            var role = new Role
            {
                Name = request.RoleName,
                Description = request.Description ?? request.RoleName.ToLower()
            };

            await _roleService.CreateAsync(role);

            return Ok();
        }
    }
}