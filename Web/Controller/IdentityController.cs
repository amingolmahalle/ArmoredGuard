using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Entities.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Services.DomainModels;
using Services.Services;
using Web.Models;

namespace Web.Controller
{
    [ApiController]
    [Route("[controller]")]
    public class IdentityController : ControllerBase
    {
        private readonly IJwtService _jwtService;

        private readonly SignInManager<User> _signInManager;

        private readonly UserManager<User> _userManager;

        private readonly IHttpContextAccessor _httpContextAccessor;

        public IdentityController(
            IJwtService jwtService,
            SignInManager<User> signInManager,
            UserManager<User> userManager,
            IHttpContextAccessor httpContextAccessor)
        {
            _jwtService = jwtService;
            _signInManager = signInManager;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// This method generate JWT Token
        /// </summary>
        /// <param name="tokenRequest">The information of token request</param>
        /// <returns></returns>
        [HttpPost("token")]
        [AllowAnonymous]
        public virtual async Task<ActionResult> Token([FromBody] TokenRequest tokenRequest)
        {
            if (!tokenRequest.GrantType.Equals("password", StringComparison.OrdinalIgnoreCase))
                throw new Exception("OAuth flow is not password.");

            //var user = await userRepository.GetByUserAndPass(username, password, cancellationToken);
            User user = await _userManager.FindByNameAsync(tokenRequest.Username);

            if (user == null)
                throw new Exception("نام کاربری یا رمز عبور اشتباه است");

            var isPasswordValid = await _userManager.CheckPasswordAsync(user, tokenRequest.Password);

            if (!isPasswordValid)
                throw new Exception("نام کاربری یا رمز عبور اشتباه است");

            var roleName = (await _userManager.GetRolesAsync(user)).Single();

            ClaimsDto tokenResult = new ClaimsDto
            {
                UserId = user.Id,
                FullName = user.FullName,
                RoleName = roleName,
                SecurityStampClaim = user.SecurityStamp
            };

            var jwt = await _jwtService.GenerateAsync(tokenResult);

            return new JsonResult(jwt);
        }

        [HttpGet("get-claims")]
        public IActionResult GetClaims()
        {
            bool result = HttpContext.User.Identity.IsAuthenticated;

            if (result)
            {
                var claims = _httpContextAccessor.HttpContext.User.Claims.ToList();

                return Ok(
                    new ClaimsDto
                    {
                        UserId = int.Parse(claims.Single(c => c.Type == ClaimTypes.NameIdentifier).Value),
                        FullName = claims.Single(c => c.Type == ClaimTypes.Name).Value,
                        RoleName = claims.Single(c => c.Type == ClaimTypes.Role).Value,
                        SecurityStampClaim =
                            claims.Single(c => c.Type == new ClaimsIdentityOptions().SecurityStampClaimType).Value
                    });
            }

            return Unauthorized();
        }

        [HttpGet("is-valid-token")]
        [AllowAnonymous]
        public bool IsValidToken()
        {
            return HttpContext.User.Identity.IsAuthenticated;
        }
    }
}