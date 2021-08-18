using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using Common.Extensions;
using Entities.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Services.Contracts;
using Web.Controller.Base;
using Web.Models.RequestModels.User;
using Web.Models.ResponseModel.User;
using WebFramework.ApiResult;

namespace Web.Controller
{
    public class UsersController : BaseController
    {
        private readonly IUserService _userService;

        private readonly IOAuthService _oAuthService;

        private readonly ILogger<UsersController> _logger;

        private readonly UserManager<User> _userManager;

        private readonly RoleManager<Role> _roleManager;

        public UsersController(
            IUserService userService,
            ILogger<UsersController> logger,
            UserManager<User> userManager,
            RoleManager<Role> roleManager,
            IOAuthService oAuthService)
        {
            _userService = userService;
            _logger = logger;
            _userManager = userManager;
            _roleManager = roleManager;
            _oAuthService = oAuthService;
        }

        [HttpGet("get-by-id/{id:int}")]
        public async Task<ApiResult<GetByUserIdResponse>> GetById(int id)
        {
            User user = await _userManager.FindByIdAsync(id.ToString());

            if (user == null)
                return NotFound();

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
        public async Task<ApiResult> Create(CreateUserRequest request, CancellationToken cancellationToken)
        {
            using (var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    bool isExistUser =
                        await _userService.IsExistUserByPhoneNumberAsync(request.PhoneNumber, cancellationToken);

                    if (isExistUser)
                        return BadRequest("user already exists");

                    _logger.LogInformation("calling create user endpoint");

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
        public async Task<ApiResult> UpdateProfile([FromRoute] int id, UpdateUserProfileRequest request,
            CancellationToken cancellationToken)
        {
            User user = await _userService.GetByIdAsync(id, cancellationToken);

            if (user == null)
                return NotFound();

            if (request?.Email != null)
            {
                user.Email = request.Email;
            }

            if (request?.PhoneNumber != null)
            {
                user.PhoneNumber = request.PhoneNumber;
            }

            if (request?.BirthDate != null)
            {
                user.BirthDate = request.BirthDate;
            }

            if (request?.FullName != null)
            {
                user.FullName = request.FullName;
            }

            if (request?.Gender != null)
            {
                user.Gender = request.Gender.GetValueOrDefault();
            }

            await _userManager.UpdateAsync(user);

            return Ok();
        }

        [HttpPut("change-password/{id:int}")]
        public async Task<ApiResult> ChangePassword([FromRoute] int id, ChangeUserPasswordRequest request,
            CancellationToken cancellationToken)
        {
            // TODO:transaction Scope

            User updateUser = await _userService.GetByIdAsync(id, cancellationToken);

            IdentityResult result =
                await _userManager.ChangePasswordAsync(updateUser, request.CurrentPassword, request.NewPassword);

            if (!result.Succeeded)
                return NotFound(
                    $"{result.Errors?.FirstOrDefault()?.Code}, {result.Errors?.FirstOrDefault()?.Description}");

            //TODO: remove refresh token

            return Ok();
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

        //TODO: Mobile Confirmed With Otp (redis)
    }
}