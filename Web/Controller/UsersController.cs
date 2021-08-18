using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using Common.Extensions;
using Data.Contracts;
using Entities.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Web.Controller.Base;
using Web.Models.RequestModels.User;
using Web.Models.ResponseModel.User;
using WebFramework.ApiResult;

namespace Web.Controller
{
    public class UsersController : BaseController
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
        public async Task<ApiResult<GetByUserIdResponse>> GetById(int id)
        {
            User user = await _userManager.FindByIdAsync(id.ToString());

            if (user == null)
                return Ok(); //?NoContent

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
        public async Task<ApiResult> Create(CreateUserRequest request)
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
        public async Task<ApiResult> UpdateProfile([FromRoute] int id, UpdateUserProfileRequest request,
            CancellationToken cancellationToken)
        {
            User user = await _userRepository.GetByIdAsync(cancellationToken, id);

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
            User updateUser = await _userRepository.GetByIdAsync(cancellationToken, id);

            IdentityResult result =
                await _userManager.ChangePasswordAsync(updateUser, request.CurrentPassword, request.NewPassword);

            if (!result.Succeeded)
                return NotFound(
                    $"{result.Errors?.FirstOrDefault()?.Code}, {result.Errors?.FirstOrDefault()?.Description}");
            
            //TODO: remove RefreshCode From table

            return Ok();
        }

        [HttpPut("inactivate/{id:int}")]
        public async Task<ApiResult> Inactivate([FromRoute] int id, CancellationToken cancellationToken)
        {
            User user = await _userRepository.GetByIdAsync(cancellationToken, id);

            if (user == null)
            {
                return NotFound();
            }

            if (!user.IsActive)
                return BadRequest("User is disabled");

            user.IsActive = false;

            await _userManager.UpdateSecurityStampAsync(user);

            return Ok();
        }

        [HttpPut("activate/{id:int}")]
        public async Task<ApiResult> Activate([FromRoute] int id, CancellationToken cancellationToken)
        {
            User user = await _userRepository.GetByIdAsync(cancellationToken, id);

            if (user == null)
            {
                return NotFound();
            }

            if (user.IsActive)
                return BadRequest("user is active");

            user.IsActive = true;

            await _userManager.UpdateSecurityStampAsync(user);

            return Ok();
        }
    }
}