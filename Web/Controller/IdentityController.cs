using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Entities.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Services;
using Services.DomainModels;
using Services.Services;
using Web.Models;

namespace Web.Controller
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
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
        public async Task<ActionResult> Token([FromBody] TokenRequest tokenRequest)
        {
            if (!tokenRequest.GrantType.Equals("password", StringComparison.OrdinalIgnoreCase))
                throw new Exception("OAuth flow is not password");

            User user = await _userManager.FindByNameAsync(tokenRequest.Username);

            if (user == null)
                return NotFound("Invalid Username or Password");

            if (!user.IsActive)
                return BadRequest("User is not active");

            bool isPasswordValid = await _userManager.CheckPasswordAsync(user, tokenRequest.Password);

            if (!isPasswordValid)
                return NotFound("Invalid Username or Password");

            IList<string> rolesName = (await _userManager.GetRolesAsync(user));

            ClaimsDto tokenResult = new ClaimsDto
            {
                UserId = user.Id,
                Username = user.UserName,
                FullName = user.FullName,
                RolesName = rolesName,
                SecurityStamp = user.SecurityStamp
            };

            AccessToken jwt = _jwtService.Generate(tokenResult);

            return new JsonResult(jwt);
        }

        [HttpGet("get-claims")]
        [Authorize]
        public IActionResult GetClaims()
        {
            bool result = HttpContext.User.Identity is {IsAuthenticated: true};

            if (!result || _httpContextAccessor.HttpContext == null)
                return Unauthorized();

            List<Claim> claims = _httpContextAccessor.HttpContext.User.Claims.ToList();

            return Ok(
                new ClaimsDto
                {
                    UserId = int.Parse(claims.Single(c => c.Type == ClaimTypes.NameIdentifier).Value),
                    Username = claims.Single(c => c.Type == ClaimTypes.Name).Value,
                    RolesName = claims.Where(c => c.Type == ClaimTypes.Role).Select(r => r.Value).ToList(),
                    SecurityStamp = claims.Single(c => c.Type == new ClaimsIdentityOptions().SecurityStampClaimType)
                        .Value
                });
        }

        [HttpGet("is-valid-token")]
        [AllowAnonymous]
        public bool IsValidToken()
        {
            return HttpContext.User.Identity is {IsAuthenticated: true};
        }
    }
}