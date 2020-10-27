using System;
using System.Threading.Tasks;
using Entities.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Services.Services;
using Web.Models;

namespace Web.Controller
{
    [ApiController]
    [Route("[controller]")]
    public class IdentityController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        
        private readonly RoleManager<Role> _roleManager;
        
        private readonly IJwtService _jwtService;
        
        private readonly SignInManager<User> _signInManager;
        
        public IdentityController(
            UserManager<User> userManager,
            RoleManager<Role> roleManager,
            IJwtService jwtService, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _jwtService = jwtService;
            _signInManager = signInManager;
        }
        
        /// <summary> This method generate JWT Token </summary>
        /// <param name="tokenRequest">The information of token request</param>
        /// <returns></returns>
        [HttpPost("[action]")]
        public virtual async Task<ActionResult> Token([FromBody] TokenRequest tokenRequest)
        {
            if (!tokenRequest.GrantType.Equals("password", StringComparison.OrdinalIgnoreCase))
                throw new Exception("OAuth flow is not password.");

            //var user = await userRepository.GetByUserAndPass(username, password, cancellationToken);
            var user = await _userManager.FindByNameAsync(tokenRequest.Username);

            if (user == null)
                throw new Exception("incorrect username or password.");

            var isPasswordValid = await _userManager.CheckPasswordAsync(user, tokenRequest.Password);

            if (!isPasswordValid)
                throw new Exception("incorrect username or password.");

            var jwt = await _jwtService.GenerateAsync(user);

            return new JsonResult(jwt);
        }
    }
}