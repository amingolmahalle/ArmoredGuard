using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using Entities.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Services.Contracts;
using Services.Dtos;
using Services.Services.Redis;
using Web.ApiResult;
using Web.Controller.Base;
using Web.Models.RequestModels.Identity;

namespace Web.Controller
{
    public class IdentityController : BaseController
    {
        private readonly IJwtService _jwtService;

        private readonly IOAuthService _oAuthService;

        private readonly SignInManager<User> _signInManager;

        private readonly IUserService _userService;

        private readonly IRedisService _redisService;

        private readonly IHttpContextAccessor _httpContextAccessor;

        public IdentityController(
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
        public async Task<IActionResult> GetTokenByUsernameAndPassword(
            [FromForm] GetTokenByUsernameAndPasswordRequest request, CancellationToken cancellationToken)
        {
            int? oAuthClientId =
                await _oAuthService.GetOAuthClientIdByClientIdAndSecretCodeAsync(request.ClientId,
                    Guid.Parse(request.ClientSecret));

            if (!oAuthClientId.HasValue)
                return Unauthorized("invalid ClientId Or SecretCode");

            User user = await _userService.FindByNameAsync(request.Username);

            if (user == null)
                return NotFound("Invalid Username or Password");

            if (!user.IsActive)
                return BadRequest("User is not active");

            bool isPasswordValid = await _userService.CheckPasswordAsync(user, request.Password);

            if (!isPasswordValid)
                return NotFound("Invalid Username or Password");

            IList<string> rolesName = (await _userService.GetRolesAsync(user));

            ClaimsDto tokenResult = new ClaimsDto
            {
                UserId = user.Id,
                Username = user.UserName,
                Roles = rolesName,
                SecurityStamp = user.SecurityStamp
            };

            AccessTokenDto accessToken = _jwtService.GenerateToken(tokenResult);

            var addRefreshTokenDto = new AddRefreshTokenDto
            {
                RefreshCode = Guid.Parse(accessToken.RefreshToken),
                UserId = user.Id,
                OAuthClientId = oAuthClientId.Value,
                CreatedAt = accessToken.CreatedAt,
                ExpireAt = accessToken.ExpiresAt
            };

            await _oAuthService.AddRefreshTokenAsync(addRefreshTokenDto, cancellationToken);

            return new JsonResult(accessToken);
        }

        [HttpPost("get-token-by-refresh-code")]
        public async Task<IActionResult> GetTokenByRefreshCode(
            GetTokenByRefreshCodeRequest request,
            CancellationToken cancellationToken)
        {
            using (var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    int? oAuthClientId =
                        await _oAuthService.GetOAuthClientIdByClientIdAndSecretCodeAsync(
                            request.ClientId,
                            Guid.Parse(request.ClientSecret));

                    if (!oAuthClientId.HasValue)
                    {
                        transactionScope.Dispose();
                        return Unauthorized("invalid ClientId Or SecretCode");
                    }

                    int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

                    OAuthRefreshToken oAuthRefreshToken =
                        await _oAuthService.GetOAuthRefreshTokenByUserIdAndRefreshCodeAndClientIdAsync(
                            userId,
                            request.RefreshToken,
                            oAuthClientId.Value);

                    if (oAuthRefreshToken == null)
                    {
                        transactionScope.Dispose();
                        return Unauthorized("invalid refresh code for this user");
                    }

                    if (oAuthRefreshToken.ExpiresAt <= DateTimeOffset.UtcNow)
                    {
                        transactionScope.Dispose();
                        return Unauthorized("refresh token has expired. please get the token again");
                    }

                    User user = await _userService.FindByIdAsync(userId.ToString());

                    if (user == null)
                    {
                        transactionScope.Dispose();
                        return NotFound("invalid username or password");
                    }

                    if (!user.IsActive)
                    {
                        transactionScope.Dispose();
                        return BadRequest("user is not active");
                    }

                    IList<string> rolesName = await _userService.GetRolesAsync(user);

                    ClaimsDto tokenResult = new ClaimsDto
                    {
                        UserId = user.Id,
                        Username = user.UserName,
                        Roles = rolesName,
                        SecurityStamp = user.SecurityStamp
                    };

                    AccessTokenDto accessToken = _jwtService.GenerateToken(tokenResult);

                    var renewRefreshTokenDto = new RenewRefreshTokenDto
                    {
                        UserId = user.Id,
                        OAuthClientId = oAuthClientId.Value,
                        OAuthRefreshTokenId = oAuthRefreshToken.Id,
                        CreatedAt = accessToken.CreatedAt,
                        ExpiresAt = accessToken.ExpiresAt,
                        NewRefreshToken = Guid.Parse(accessToken.RefreshToken)
                    };

                    await _oAuthService.RenewRefreshTokenAsync(renewRefreshTokenDto, cancellationToken);

                    transactionScope.Complete();

                    return new JsonResult(accessToken);
                }
                catch (Exception e)
                {
                    transactionScope.Dispose();
                    throw new Exception(e.Message, e.InnerException);
                }
            }
        }

        [HttpPost("get-token-by-otp")]
        public async Task<IActionResult> GetTokenByOtp(
            GetTokenByOtpRequest request,
            CancellationToken cancellationToken)
        {
            using (var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    int? oAuthClientId =
                        await _oAuthService.GetOAuthClientIdByClientIdAndSecretCodeAsync(
                            request.ClientId,
                            Guid.Parse(request.ClientSecret));

                    if (!oAuthClientId.HasValue)
                    {
                        transactionScope.Dispose();
                        return Unauthorized("invalid ClientId Or SecretCode");
                    }

                    string otpCode = await _redisService.GetAsync<string>(request.PhoneNumber, cancellationToken);

                    if (string.IsNullOrEmpty(otpCode))
                    {
                        transactionScope.Dispose();
                        return Unauthorized("No otp code found for this phone number");
                    }

                    if (otpCode != request.OtpCode.Trim())
                    {
                        transactionScope.Dispose();
                        return Unauthorized($" otp code {request.OtpCode} for this phone number is invalid");
                    }
                    
                    User user = await _userService.GetByPhoneNumberAsync(request.PhoneNumber, cancellationToken);

                    if (user == null)
                    {
                        transactionScope.Dispose();
                        return NotFound("invalid username or password");
                    }

                    if (!user.IsActive)
                    {
                        transactionScope.Dispose();
                        return BadRequest("user is not active");
                    }

                    IList<string> rolesName = await _userService.GetRolesAsync(user);

                    ClaimsDto tokenResult = new ClaimsDto
                    {
                        UserId = user.Id,
                        Username = user.UserName,
                        Roles = rolesName,
                        SecurityStamp = user.SecurityStamp
                    };

                    AccessTokenDto accessToken = _jwtService.GenerateToken(tokenResult);

                    var addRefreshTokenDto = new AddRefreshTokenDto
                    {
                        RefreshCode = Guid.Parse(accessToken.RefreshToken),
                        UserId = user.Id,
                        OAuthClientId = oAuthClientId.Value,
                        CreatedAt = accessToken.CreatedAt,
                        ExpireAt = accessToken.ExpiresAt
                    };

                    await _oAuthService.AddRefreshTokenAsync(addRefreshTokenDto, cancellationToken);
                    
                    await _redisService.RemoveAsync(request.PhoneNumber, cancellationToken);

                    transactionScope.Complete();

                    return new JsonResult(accessToken);
                }
                catch (Exception e)
                {
                    transactionScope.Dispose();
                    throw new Exception(e.Message, e.InnerException);
                }
            }
        }

        [HttpGet("get-claims")]
        public ApiResult<ClaimsDto> GetClaims()
        {
            if (_httpContextAccessor.HttpContext == null)
                return BadRequest();

            List<Claim> claims = _httpContextAccessor.HttpContext.User.Claims.ToList();

            return
                new ClaimsDto
                {
                    UserId = int.Parse(claims.Single(c => c.Type == ClaimTypes.NameIdentifier).Value),
                    Username = claims.Single(c => c.Type == ClaimTypes.Name).Value,
                    Roles = claims.Where(c => c.Type == ClaimTypes.Role).Select(r => r.Value).ToList(),
                    SecurityStamp = claims.Single(c => c.Type == new ClaimsIdentityOptions().SecurityStampClaimType)
                        .Value
                };
        }

        [HttpGet("is-valid-token")]
        public ApiResult<object> IsValidToken()
        {
            return Ok(HttpContext.User.Identity is {IsAuthenticated: true});
        }

        //TODO: Logout Endpoint
    }
}