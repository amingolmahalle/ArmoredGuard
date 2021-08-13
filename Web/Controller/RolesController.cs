using System.Threading;
using System.Threading.Tasks;
using Entities.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Web.Models.Role;

namespace Web.Controller
{
    [ApiController]
    [Route("[controller]")]
    public class RolesController : ControllerBase
    {
        private readonly RoleManager<Role> _roleManager;

        public RolesController(RoleManager<Role> roleManager)
        {
            _roleManager = roleManager;
        }

        [HttpPost("create")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Create(CreateRoleRequest request, CancellationToken cancellationToken)
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