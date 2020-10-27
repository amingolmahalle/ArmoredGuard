using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Data.Contracts;
using Entities.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Web.Models;

namespace Web.Controller
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;

        private readonly ILogger<UserController> _logger;

        private readonly UserManager<User> _userManager;

        public UserController(
            IUserRepository userRepository,
            ILogger<UserController> logger,
            UserManager<User> userManager)
        {
            _userRepository = userRepository;
            _logger = logger;
            _userManager = userManager;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public virtual async Task<ActionResult<List<User>>> GetAll(CancellationToken cancellationToken)
        {
            //var userName = HttpContext.User.Identity.GetUserName();
            //userName = HttpContext.User.Identity.Name;
            //var userId = HttpContext.User.Identity.GetUserId();
            //var userIdInt = HttpContext.User.Identity.GetUserId<int>();
            //var phone = HttpContext.User.Identity.FindFirstValue(ClaimTypes.MobilePhone);
            //var role = HttpContext.User.Identity.FindFirstValue(ClaimTypes.Role);

            var users = await _userRepository.TableNoTracking.ToListAsync(cancellationToken);
            
            return Ok(users);
        }

        [HttpGet("{id:int}")]
        [Authorize(Roles = "Admin")]
        public virtual async Task<ActionResult<User>> GetById(int id, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());

            if (user == null)
                return NotFound();

            await _userManager.UpdateSecurityStampAsync(user);
            //await userRepository.UpdateSecurityStampAsync(user, cancellationToken);

            return user;
        }

        [HttpPost]
        [AllowAnonymous]
        public virtual async Task<User> Add([FromBody] UserDto userDto)
        {
            _logger.LogError("Call Create Action ...");

            var user = new User
            {
                Age = userDto.Age,
                FullName = userDto.FullName,
                Gender = userDto.Gender,
                UserName = userDto.UserName,
                Email = userDto.Email
            };
            await _userManager.CreateAsync(user, userDto.Password);

            // await _roleManager.CreateAsync(new Role
            // {
            //     Name = "Admin",
            //     Description = "Admin role"
            // });

            await _userManager.AddToRoleAsync(user, "Admin");

            return user;
        }

        [HttpPut]
        [Authorize(Roles = "Admin")]
        public virtual async Task<ActionResult> Edit(int id, User user, CancellationToken cancellationToken)
        {
            var updateUser = await _userRepository.GetByIdAsync(cancellationToken, id);

            updateUser.UserName = user.UserName;
            updateUser.PasswordHash = user.PasswordHash;
            updateUser.FullName = user.FullName;
            updateUser.Age = user.Age;
            updateUser.Gender = user.Gender;
            updateUser.IsActive = user.IsActive;
            updateUser.LastLoginDate = user.LastLoginDate;

            await _userRepository.UpdateAsync(updateUser, cancellationToken);

            return Ok();
        }

        [HttpDelete]
        [Authorize(Roles = "Admin")]
        public virtual async Task<ActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(cancellationToken, id);
            await _userRepository.DeleteAsync(user, cancellationToken);

            return Ok();
        }
    }
}