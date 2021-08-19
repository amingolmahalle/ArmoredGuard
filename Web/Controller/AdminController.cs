using System.Threading;
using System.Threading.Tasks;
using Entities.Entity;
using Microsoft.AspNetCore.Mvc;
using Services.Contracts;
using Web.Controller.Base;

namespace Web.Controller
{
    public class AdminController : BaseController
    {
        private readonly IUserService _userService;

        public AdminController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPut("inactivate/{id:int}")]
        public async Task<ApiResult.ApiResult> Inactivate([FromRoute] int id, CancellationToken cancellationToken)
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

            await _userService.UpdateSecurityStampAsync(user);

            //TODO: remove refresh token

            return Ok();
        }

        [HttpPut("activate/{id:int}")]
        public async Task<ApiResult.ApiResult> Activate([FromRoute] int id, CancellationToken cancellationToken)
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

            await _userService.UpdateSecurityStampAsync(user);

            //TODO: remove refresh token

            return Ok();
        }
    }
}