using System.Threading;
using System.Threading.Tasks;
using Entities.Entity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Services.Contracts;
using Web.Controller.Base;
using WebFramework.ApiResult;

namespace Web.Controller
{
    public class AdminController : BaseController
    {
        private readonly IUserService _userService;

        private readonly UserManager<User> _userManager;

        public AdminController(IUserService userService, UserManager<User> userManager)
        {
            _userService = userService;
            _userManager = userManager;
        }

        [HttpPut("inactivate/{id:int}")]
        public async Task<ApiResult> Inactivate([FromRoute] int id, CancellationToken cancellationToken)
        {
            // TODO:transaction Scope
            User user = await _userService.GetByIdAsync(id, cancellationToken);

            if (user == null)
            {
                return NotFound();
            }

            if (!user.IsActive)
                return BadRequest("User is disabled");

            user.IsActive = false;

            await _userManager.UpdateSecurityStampAsync(user);

            //TODO: remove refresh token

            return Ok();
        }

        [HttpPut("activate/{id:int}")]
        public async Task<ApiResult> Activate([FromRoute] int id, CancellationToken cancellationToken)
        {
            // TODO:transaction Scope
            User user = await _userService.GetByIdAsync(id, cancellationToken);

            if (user == null)
            {
                return NotFound();
            }

            if (user.IsActive)
                return BadRequest("user is active");

            user.IsActive = true;

            await _userManager.UpdateSecurityStampAsync(user);

            //TODO: remove refresh token

            return Ok();
        }
    }
}