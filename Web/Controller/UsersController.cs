using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using Common.Exceptions;
using Common.Extensions;
using Entities.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Services.Contracts;
using Services.Dtos;
using Web.ApiResult;
using Web.Controller.Base;
using Web.Models.RequestModels.User;
using Web.Models.ResponseModel.User;

namespace Web.Controller
{
    public class UsersController : BaseController
    {
        private readonly IUserService _userService;

        private readonly IOAuthService _oAuthService;

        private readonly ILogger<UsersController> _logger;

        private readonly IRoleService _roleService;

        public UsersController(
            IUserService userService,
            ILogger<UsersController> logger,
            IOAuthService oAuthService,
            IRoleService roleService)
        {
            _userService = userService;
            _logger = logger;
            _oAuthService = oAuthService;
            _roleService = roleService;
        }

        [HttpGet("get-by-id/{id:int}")]
        public async Task<ApiResult<GetByUserIdResponse>> GetById(int id)
        {
            User user = await _userService.FindByIdAsync(id.ToString());

            if (user == null)
                throw new NotFoundException("user not found");

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
        public async Task<ApiResult.ApiResult> Create(CreateUserRequest request, CancellationToken cancellationToken)
        {
            using (var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    bool isExistUser =
                        await _userService.IsExistByPhoneNumberAsync(request.PhoneNumber, cancellationToken);

                    if (isExistUser)
                        throw new BadRequestException("user already exists");

                    _logger.LogInformation("calling create user endpoint");

                    if (request.RoleId == 0)
                        throw new BadRequestException("RoleId is Invalid");

                    var role = await _roleService.FindByIdAsync(request.RoleId.ToString());

                    if (role == null)
                        throw new BadRequestException("RoleId is Invalid");

                    var createUserDto = new CreateUserDto
                    {
                        UserName = request.UserName,
                        Email = request.Email.ToFormalEmail(),
                        PhoneNumber = request.PhoneNumber.ToFormalPhoneNumber(),
                        Password = request.Password,
                        OtpCode = request.OtpCode,
                        FullName = request.FullName,
                        RegisterType = request.RegisterType,
                        RoleId = request.RoleId,
                        BirthDate = request.BirthDate,
                        Gender = request.Gender
                    };

                    User user = await _userService.CreateAsync(createUserDto, cancellationToken);

                    await _userService.AddToRoleAsync(user, role.Name);

                    transactionScope.Complete();

                    return Ok();
                }
                catch (Exception)
                {
                    transactionScope.Dispose();

                    return null;
                }
            }
        }

        [HttpPut("update-profile/{id:int}")]
        public async Task<ApiResult.ApiResult> UpdateProfile([FromRoute] int id, UpdateUserProfileRequest request)
        {
            User user = await _userService.FindByIdAsync(id.ToString());

            if (user == null)
                throw new NotFoundException("user not found");

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

            await _userService.UpdateAsync(user);

            return Ok();
        }

        [HttpPut("change-password/{id:int}")]
        public async Task<ApiResult.ApiResult> ChangePassword([FromRoute] int id, ChangeUserPasswordRequest request,
            CancellationToken cancellationToken)
        {
            using (var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    User user = await _userService.FindByIdAsync(id.ToString());

                    if (user == null)
                        throw new NotFoundException("user not found");

                    IdentityResult result =
                        await _userService.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);

                    if (!result.Succeeded)
                        throw new NotFoundException(
                            $"{result.Errors?.FirstOrDefault()?.Code}, {result.Errors?.FirstOrDefault()?.Description}");

                    await _oAuthService.DeleteAllUserRefreshCodesAsync(user.Id, cancellationToken);

                    return Ok();
                }
                catch (Exception)
                {
                    transactionScope.Dispose();

                    return null;
                }
            }
        }
    }
}