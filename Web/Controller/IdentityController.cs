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
using Services.DomainModels;
using Services.Dtos;
using Services.Services;
using Web.Controller.Base;
using Web.Models;
using WebFramework.ApiResult;

namespace Web.Controller
{
    public class IdentityController : BaseController
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
        public async Task<ActionResult> Token([FromForm] TokenRequest tokenRequest)
        {
            if (!tokenRequest.grant_type.Equals("password", StringComparison.OrdinalIgnoreCase))
                throw new Exception("OAuth flow is not password");

            User user = await _userManager.FindByNameAsync(tokenRequest.username);

            if (user == null)
                return NotFound("Invalid Username or Password");

            if (!user.IsActive)
                return BadRequest("User is not active");

            bool isPasswordValid = await _userManager.CheckPasswordAsync(user, tokenRequest.password);

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

            AccessTokenDto jwt = _jwtService.Generate(tokenResult);

            return new JsonResult(jwt);
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
    }
}