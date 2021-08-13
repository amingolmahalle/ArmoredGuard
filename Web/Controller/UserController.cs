using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Data.Contracts;
using Entities.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Web.Models;

namespace Web.Controller
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;

        private readonly ILogger<UserController> _logger;

        private readonly UserManager<User> _userManager;

        private readonly RoleManager<Role> _roleManager;

        public UserController(
            IUserRepository userRepository,
            ILogger<UserController> logger,
            UserManager<User> userManager,
            RoleManager<Role> roleManager)
        {
            _userRepository = userRepository;
            _logger = logger;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [HttpGet("/get-by-id/{id:int}")]
        [Authorize(Roles = "Admin")]
        public virtual async Task<ActionResult<GetByUserIdResponse>> GetById(int id)
        {
            User user = await _userManager.FindByIdAsync(id.ToString());

            if (user == null)
                return NoContent();

            var getByUserIdResponse = new GetByUserIdResponse
            {
                FullName = user.FullName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                BirthDate = user.BirthDate,
                Gender = user.Gender,
                UserName = user.UserName,
                LastLoginDate = user.LastLoginDate.GetValueOrDefault()
            };

            return getByUserIdResponse;
        }

        [HttpPost("create")]
        [AllowAnonymous]
        public virtual async Task Create(CreateUserRequest userRequest)
        {
            _logger.LogInformation("calling Create User Action");

            var user = new User
            {
                BirthDate = userRequest.BirthDate,
                FullName = userRequest.FullName,
                Gender = userRequest.Gender,
                UserName = userRequest.UserName,
                Email = userRequest.Email
            };
            await _userManager.CreateAsync(user, userRequest.Password);

            await _roleManager.CreateAsync(new Role
            {
                Name = "Admin",
                Description = "admin role"
            });

            await _userManager.AddToRoleAsync(user, "Admin");
        }

        [HttpPut("update-profile")]
        [Authorize(Roles = "Admin")]
        public virtual async Task UpdateProfile(int id, UpdateUserProfileRequest request,
            CancellationToken cancellationToken)
        {
            User updateUser = await _userRepository.GetByIdAsync(cancellationToken, id);

            if (request?.UserName != null)
            {
                updateUser.UserName = request.UserName;
            }

            if (request?.Email != null)
            {
                updateUser.Email = request.Email;
            }

            if (request?.PhoneNumber != null)
            {
                updateUser.PhoneNumber = request.PhoneNumber;
            }

            if (request?.BirthDate != null)
            {
                updateUser.BirthDate = request.BirthDate;
            }

            if (request?.FullName != null)
            {
                updateUser.FullName = request.FullName;
            }

            if (request?.Gender != null)
            {
                updateUser.Gender = request.Gender.GetValueOrDefault();
            }

            await _userManager.UpdateSecurityStampAsync(updateUser);
        }

        [HttpPut("change-password")]
        [Authorize(Roles = "Admin")]
        public virtual async Task<IActionResult> ChangePassword(int id, ChangeUserPasswordRequest request,
            CancellationToken cancellationToken)
        {
            User updateUser = await _userRepository.GetByIdAsync(cancellationToken, id);

            IdentityResult result =
                await _userManager.ChangePasswordAsync(updateUser, request.CurrentPassword, request.NewPassword);

            if (!result.Succeeded)
                return NotFound(
                    $"{result.Errors?.FirstOrDefault()?.Code}, {result.Errors?.FirstOrDefault()?.Description}");

            return Ok();
        }

        [HttpPut("inactivate/{id}")]
        [Authorize(Roles = "Admin")]
        public virtual async Task<ActionResult> Inactivate([FromRoute] int id, CancellationToken cancellationToken)
        {
            User updateUser = await _userRepository.GetByIdAsync(cancellationToken, id);

            if (updateUser == null)
            {
                return NoContent();
            }

            if (!updateUser.IsActive)
                return BadRequest("User is disabled");

            updateUser.IsActive = false;

            await _userManager.UpdateSecurityStampAsync(updateUser);

            return Ok();
        }

        [HttpPut("activate/{id}")]
        [Authorize(Roles = "Admin")]
        public virtual async Task<ActionResult> Activate([FromRoute] int id, CancellationToken cancellationToken)
        {
            User updateUser = await _userRepository.GetByIdAsync(cancellationToken, id);

            if (updateUser == null)
            {
                return NoContent();
            }

            if (updateUser.IsActive)
                return BadRequest("user is active");

            updateUser.IsActive = true;

            await _userManager.UpdateSecurityStampAsync(updateUser);

            return Ok();
        }
    }
}