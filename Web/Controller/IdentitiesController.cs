using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using Common.Exceptions;
using Entities.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Services.Contracts;
using Services.Contracts.Redis;
using Services.Dtos;
using Web.ApiResult;
using Web.Controller.Base;
using Web.Models.RequestModels.Identity;

namespace Web.Controller
{
    public class IdentitiesController : BaseController
    {
        private readonly IJwtService _jwtService;

        private readonly IOAuthService _oAuthService;

        private readonly SignInManager<User> _signInManager;

        private readonly IUserService _userService;

        private readonly IRedisService _redisService;

        private readonly IHttpContextAccessor _httpContextAccessor;

        public IdentitiesController(
            IJwtService jwtService,
            SignInManager<User> signInManager,
            IOAuthService authService,
            IRedisService redisService,
            IUserService userService,
            IHttpContextAccessor httpContextAccessor)
        {
            _jwtService = jwtService;
            _signInManager = signInManager;
            _redisService = redisService;
            _oAuthService = authService;
            _userService = userService;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpPost("get-token-by-username-and-password")]
        [AllowAnonymous]
        public async Task<AccessTokenDto> GetTokenByUsernameAndPassword(
            [FromForm] GetTokenByUsernameAndPasswordRequest request,
            CancellationToken cancellationToken)
        {
            int? oAuthClientId =
                await _oAuthService.GetOAuthClientIdByClientIdAndSecretCodeAsync(request.client_id,
                    Guid.Parse(request.client_secret), cancellationToken);

            if (!oAuthClientId.HasValue)
                throw new UnAuthorizedException("invalid ClientId Or SecretCode");

            User user = await _userService.FindByNameAsync(request.username);

            if (user == null)
                throw new NotFoundException("Invalid Username or Password");

            if (!user.IsActive)
                throw new BadRequestException("User is not active");

            bool isPasswordValid = await _userService.CheckPasswordAsync(user, request.password);

            if (!isPasswordValid)
                throw new NotFoundException("Invalid Username or Password");

            IList<string> rolesName = (await _userService.GetRolesAsync(user));

            ClaimsDto tokenResult = new ClaimsDto
            {
                UserId = user.Id,
                Username = user.UserName,
                FirstName = user.FullName,
                LastName = user.FullName,
                MobileNumber = user.PhoneNumber,
                Roles = rolesName,
                SecurityStamp = user.SecurityStamp
            };

            AccessTokenDto accessToken = _jwtService.GenerateToken(tokenResult);

            var addRefreshTokenDto = new AddRefreshTokenDto
            {
                RefreshCode = Guid.Parse(accessToken.refresh_token),
                UserId = user.Id,
                OAuthClientId = oAuthClientId.Value,
                CreatedAt = accessToken.Created_at,
                ExpireAt = accessToken.Expires_at
            };

            await _oAuthService.AddRefreshTokenAsync(addRefreshTokenDto, cancellationToken);

            return accessToken;
        }

        [HttpPost("get-token-by-refresh-code")]
        public async Task<AccessTokenDto> GetTokenByRefreshCode(
            GetTokenByRefreshCodeRequest request,
            CancellationToken cancellationToken)
        {
            using (var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    int? oAuthClientId =
                        await _oAuthService.GetOAuthClientIdByClientIdAndSecretCodeAsync(
                            request.client_id,
                            Guid.Parse(request.client_secret), cancellationToken);

                    if (!oAuthClientId.HasValue)
                        throw new UnAuthorizedException("invalid ClientId Or SecretCode");

                    int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

                    OAuthRefreshToken oAuthRefreshToken =
                        await _oAuthService.GetOAuthRefreshTokenByUserIdAndRefreshCodeAndClientIdAsync(
                            userId,
                            request.refresh_token,
                            oAuthClientId.Value,
                            cancellationToken);

                    if (oAuthRefreshToken == null)
                        throw new UnAuthorizedException("invalid refresh code for this user");

                    if (oAuthRefreshToken.ExpiresAt <= DateTimeOffset.UtcNow)
                        throw new UnAuthorizedException("refresh token has expired. please get the token again");

                    User user = await _userService.FindByIdAsync(userId.ToString());

                    if (user == null)
                        throw new NotFoundException("invalid username or password");

                    if (!user.IsActive)
                        throw new BadRequestException("user is not active");

                    IList<string> rolesName = await _userService.GetRolesAsync(user);

                    ClaimsDto tokenResult = new ClaimsDto
                    {
                        UserId = user.Id,
                        Username = user.UserName,
                        FirstName = user.FullName,
                        LastName = user.FullName,
                        MobileNumber = user.PhoneNumber,
                        Roles = rolesName,
                        SecurityStamp = user.SecurityStamp
                    };

                    AccessTokenDto accessToken = _jwtService.GenerateToken(tokenResult);

                    var renewRefreshTokenDto = new RenewRefreshTokenDto
                    {
                        UserId = user.Id,
                        OAuthClientId = oAuthClientId.Value,
                        OAuthRefreshTokenId = oAuthRefreshToken.Id,
                        CreatedAt = accessToken.Created_at,
                        ExpiresAt = accessToken.Expires_at,
                        NewRefreshToken = Guid.Parse(accessToken.refresh_token)
                    };

                    await _oAuthService.RenewRefreshTokenAsync(renewRefreshTokenDto, cancellationToken);

                    transactionScope.Complete();

                    return accessToken;
                }
                catch (Exception)
                {
                    transactionScope.Dispose();

                    throw;
                }
            }
        }

        [HttpPost("get-token-by-otp")]
        [AllowAnonymous]
        public async Task<AccessTokenDto> GetTokenByOtp(
            GetTokenByOtpRequest request,
            CancellationToken cancellationToken)
        {
            using (var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    int? oAuthClientId =
                        await _oAuthService.GetOAuthClientIdByClientIdAndSecretCodeAsync(
                            request.client_id,
                            Guid.Parse(request.client_secret), cancellationToken);

                    if (!oAuthClientId.HasValue)
                        throw new UnAuthorizedException("invalid ClientId Or SecretCode");

                    SendOtpDto otpDto = await _redisService.GetAsync<SendOtpDto>(request.phone_number, cancellationToken);

                    if (string.IsNullOrEmpty(otpDto.OtpCode))
                        throw new UnAuthorizedException("No otp code found for this phone number");

                    if (otpDto.OtpCode != request.otp_code.Trim())
                        throw new UnAuthorizedException(
                            $" otp code {request.otp_code} for this phone number is invalid");

                    User user = await _userService.GetByPhoneNumberAsync(request.phone_number, cancellationToken);

                    if (user == null)
                        throw new NotFoundException("invalid username or password");

                    if (!user.IsActive)
                        throw new BadRequestException("user is not active");

                    IList<string> rolesName = await _userService.GetRolesAsync(user);

                    ClaimsDto tokenResult = new ClaimsDto
                    {
                        UserId = user.Id,
                        Username = user.UserName,
                        FirstName = user.FullName,
                        LastName = user.FullName,
                        MobileNumber = user.PhoneNumber,
                        Roles = rolesName,
                        SecurityStamp = user.SecurityStamp
                    };

                    AccessTokenDto accessToken = _jwtService.GenerateToken(tokenResult);

                    var addRefreshTokenDto = new AddRefreshTokenDto
                    {
                        RefreshCode = Guid.Parse(accessToken.refresh_token),
                        UserId = user.Id,
                        OAuthClientId = oAuthClientId.Value,
                        CreatedAt = accessToken.Created_at,
                        ExpireAt = accessToken.Expires_at
                    };

                    await _oAuthService.AddRefreshTokenAsync(addRefreshTokenDto, cancellationToken);

                    await _redisService.RemoveAsync(request.phone_number, cancellationToken);

                    transactionScope.Complete();

                    return accessToken;
                }
                catch (Exception)
                {
                    transactionScope.Dispose();

                    throw;
                }
            }
        }

        [HttpGet("get-claims")]
        public ApiResult<ClaimsDto> GetClaims()
        {
            if (_httpContextAccessor.HttpContext == null)
                throw new BadRequestException("request is invalid");

            List<Claim> claims = _httpContextAccessor.HttpContext.User.Claims.ToList();

            ClaimsDto claimsDto = new ClaimsDto
            {
                UserId = int.Parse(claims.Single(c => c.Type == ClaimTypes.NameIdentifier).Value),
                Username = claims.Single(c => c.Type == ClaimTypes.Name).Value,
                FirstName = claims.Single(c => c.Type == "FirstName").Value,
                LastName = claims.Single(c => c.Type == "LastName").Value,
                MobileNumber = claims.Single(c => c.Type == "MobileNumber").Value,
                Roles = claims.Where(c => c.Type == ClaimTypes.Role).Select(r => r.Value).ToList(),
                SecurityStamp = claims.Single(c => c.Type == new ClaimsIdentityOptions().SecurityStampClaimType)
                    .Value
            };

            return claimsDto;
        }

        [HttpGet("is-valid-token")]
        public ApiResult<object> IsValidToken()
        {
            return Ok(HttpContext.User.Identity is {IsAuthenticated: true});
        }

        [HttpGet("logout")]
        public async Task<ApiResult.ApiResult> Logout(CancellationToken cancellationToken)
        {
            using (var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

                    User user = await _userService.FindByIdAsync(userId.ToString());

                    if (user == null)
                        throw new NotFoundException("user not found");

                    await _userService.UpdateSecurityStampAsync(user);
                    await _oAuthService.DeleteAllUserRefreshCodesAsync(userId, cancellationToken);

                    transactionScope.Complete();

                    return Ok();
                }
                catch (Exception)
                {
                    transactionScope.Dispose();

                    throw;
                }
            }
        }
    }
}
