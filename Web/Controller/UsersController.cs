using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using Common.Helpers;
using Data.Contracts;
using Entities.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Web.Models.RequestModels.User;
using Web.Models.ResponseModel.User;

namespace Web.Controller
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _userRepository;

        private readonly ILogger<UsersController> _logger;

        private readonly UserManager<User> _userManager;

        private readonly RoleManager<Role> _roleManager;

        public UsersController(
            IUserRepository userRepository,
            ILogger<UsersController> logger,
            UserManager<User> userManager,
            RoleManager<Role> roleManager)
        {
            _userRepository = userRepository;
            _logger = logger;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [HttpGet("get-by-id/{id:int}")]
        public async Task<ActionResult<GetByUserIdResponse>> GetById(int id)
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
                LastLoginDate = user.LastSeenDate.GetValueOrDefault()
            };

            return getByUserIdResponse;
        }

        [HttpPost("create")]
        [AllowAnonymous]
        public async Task<IActionResult> Create(CreateUserRequest request)
        {
            using (var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    _logger.LogInformation("calling Create User Action");

                    var user = new User
                    {
                        BirthDate = request.BirthDate,
                        FullName = request.FullName,
                        Gender = request.Gender,
                        UserName = request.UserName,
                        Email = request.Email.ToFormalEmail(),
                        PhoneNumber = request.PhoneNumber.ToFormalPhoneNumber()
                    };
                    await _userManager.CreateAsync(user, request.Password);

                    if (request.RoleId == 0)
                    {
                        transactionScope.Dispose();
                        return BadRequest("RoleId is Invalid");
                    }

                    var role = await _roleManager.FindByIdAsync(request.RoleId.ToString());

                    if (role == null)
                    {
                        transactionScope.Dispose();
                        return BadRequest("RoleId is Invalid");
                    }

                    await _userManager.AddToRoleAsync(user, role.Name);

                    transactionScope.Complete();

                    return Ok();
                }

                catch (Exception e)
                {
                    transactionScope.Dispose();
                    throw new Exception(e.Message, e.InnerException);
                }
            }
        }

        [HttpPut("update-profile/{id:int}")]
        public async Task UpdateProfile([FromRoute] int id, UpdateUserProfileRequest request,
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

        [HttpPut("change-password/{id:int}")]
        public async Task<IActionResult> ChangePassword([FromRoute] int id, ChangeUserPasswordRequest request,
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

        [HttpPut("inactivate/{id:int}")]
        public async Task<ActionResult> Inactivate([FromRoute] int id, CancellationToken cancellationToken)
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

        [HttpPut("activate/{id:int}")]
        public async Task<ActionResult> Activate([FromRoute] int id, CancellationToken cancellationToken)
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