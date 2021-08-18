using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using Entities.OAuth;
using Entities.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Services.DomainModels;
using Services.Dtos;
using Services.Services;
using Web.Controller.Base;
using Web.Models.RequestModels.Identity;
using WebFramework.ApiResult;

namespace Web.Controller
{
    public class IdentityController : BaseController
    {
        private readonly IJwtService _jwtService;
        private readonly IOAuthService _oAuthService;

        private readonly SignInManager<User> _signInManager;

        private readonly UserManager<User> _userManager;

        private readonly IHttpContextAccessor _httpContextAccessor;

        public IdentityController(
            IJwtService jwtService,
            SignInManager<User> signInManager,
            UserManager<User> userManager,
            IHttpContextAccessor httpContextAccessor, IOAuthService authService)
        {
            _jwtService = jwtService;
            _signInManager = signInManager;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
            _oAuthService = authService;
        }

        [HttpPost("get-token-by-username-and-password")]
        [AllowAnonymous]
        public async Task<IActionResult> GetTokenByUsernameAndPassword(
            [FromForm] GetTokenByUsernameAndPasswordRequest request)
        {
            bool isApplicantValidAsync =
                await _oAuthService.IsApplicantValidAsync(request.client_id, request.client_secret);

            if (!isApplicantValidAsync)
                return Unauthorized("invalid ClientId Or SecretCode");

            User user = await _userManager.FindByNameAsync(request.username);

            if (user == null)
                return NotFound("Invalid Username or Password");

            if (!user.IsActive)
                return BadRequest("User is not active");

            bool isPasswordValid = await _userManager.CheckPasswordAsync(user, request.password);

            if (!isPasswordValid)
                return NotFound("Invalid Username or Password");

            IList<string> rolesName = (await _userManager.GetRolesAsync(user));

            ClaimsDto tokenResult = new ClaimsDto
            {
                UserId = user.Id,
                Username = user.UserName,
                Roles = rolesName,
                SecurityStamp = user.SecurityStamp
            };

            AccessTokenDto accessToken = _jwtService.Generate(tokenResult);

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
                    int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

                    int? oAuthClientId =
                        await _oAuthService.GetOAuthClientIdByClientIdAndSecretCodeAsync(
                            request.client_id,
                            request.client_secret);

                    if (!oAuthClientId.HasValue)
                    {
                        transactionScope.Dispose();
                        return Unauthorized("invalid ClientId Or SecretCode");
                    }

                    OAuthRefreshToken oAuthRefreshToken =
                        await _oAuthService.GetOAuthRefreshTokenByUserIdAndRefreshCodeAndClientIdAsync(
                            userId,
                            request.refresh_token,
                            oAuthClientId.Value);

                    if (oAuthRefreshToken == null)
                    {
                        transactionScope.Dispose();
                        return Unauthorized("invalid refresh code for user");
                    }

                    if (oAuthRefreshToken.ExpiresAt >= oAuthRefreshToken.CreatedAt)
                    {
                        transactionScope.Dispose();
                        return Unauthorized("refresh token has expired. please get the token again");
                    }

                    User user = await _userManager.FindByIdAsync(userId.ToString());

                    if (user == null)
                    {
                        transactionScope.Dispose();
                        return NotFound("invalid username or password");
                    }

                    if (!user.IsActive)
                    {
                        transactionScope.Dispose();
                        return BadRequest("User is not active");
                    }

                    IList<string> rolesName = await _userManager.GetRolesAsync(user);

                    ClaimsDto tokenResult = new ClaimsDto
                    {
                        UserId = user.Id,
                        Username = user.UserName,
                        Roles = rolesName,
                        SecurityStamp = user.SecurityStamp
                    };

                    AccessTokenDto accessToken = _jwtService.Generate(tokenResult);

                    var renewRefreshTokenDto = new RenewRefreshTokenDto
                    {
                        UserId = user.Id,
                        OAuthClientId = oAuthClientId.Value,
                        OAuthRefreshTokenId = oAuthRefreshToken.Id,
                        CreatedAt = accessToken.CreatedAt,
                        ExpiresAt = accessToken.ExpiresAt,
                        NewRefreshToken = Guid.Parse(accessToken.refresh_token)
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
        [AllowAnonymous]
        public ApiResult<object> IsValidToken()
        {
            return Ok(HttpContext.User.Identity is {IsAuthenticated: true});
        }
        
        //TODO: Logout Endpoint
    }
}