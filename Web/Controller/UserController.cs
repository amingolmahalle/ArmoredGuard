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

        private readonly RoleManager<Role> _roleManager;

        public UserController(
            IUserRepository userRepository,
            ILogger<UserController> logger,
            UserManager<User> userManager,
            RoleManager<Role> roleManager)
        {
            _userRepository = userRepository;
            _logger = logger;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public virtual async Task<ActionResult<List<User>>> Get(CancellationToken cancellationToken)
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
        public virtual async Task<ActionResult<User>> Get(int id, CancellationToken cancellationToken)
        {
            var user2 = await _userManager.FindByIdAsync(id.ToString());
            var role = await _roleManager.FindByNameAsync("Admin");

            var user = await _userRepository.GetByIdAsync(cancellationToken, id);
            if (user == null)
                return NotFound();

            await _userManager.UpdateSecurityStampAsync(user);
            //await userRepository.UpdateSecurityStampAsync(user, cancellationToken);

            return user;
        }

        [HttpPost("create")]
        [AllowAnonymous]
        public virtual async Task<User> Create(UserRequest userRequest, CancellationToken cancellationToken)
        {
            _logger.LogInformation("متد Create فراخوانی شد");

            //var exists = await userRepository.TableNoTracking.AnyAsync(p => p.UserName == userDto.UserName);
            
            //if (exists)
            //    return BadRequest("نام کاربری تکراری است");


            var user = new User
            {
                // Age = userRequest.Age,
                FullName = userRequest.FullName,
                Gender = userRequest.Gender,
                UserName = userRequest.UserName,
                Email = userRequest.Email
            };
            await _userManager.CreateAsync(user, userRequest.Password);

            await _roleManager.CreateAsync(new Role
            {
                Name = "Admin",
                Description = "admin role"
            });

            await _userManager.AddToRoleAsync(user, "Admin");

            //await userRepository.AddAsync(user, userDto.Password, cancellationToken);
            return user;
        }

        [HttpPut]
        [Authorize(Roles = "Admin")]
        public virtual async Task<ActionResult> Update(int id, User user, CancellationToken cancellationToken)
        {
            var updateUser = await _userRepository.GetByIdAsync(cancellationToken, id);

            updateUser.UserName = user?.UserName;
            updateUser.PasswordHash = user?.PasswordHash;
            updateUser.FullName = user?.FullName;
            // updateUser.Age = user.Age;
            updateUser.Gender = user.Gender;
            updateUser.IsActive = user.IsActive;
            // updateUser.LastLoginDate = user.LastLoginDate;

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