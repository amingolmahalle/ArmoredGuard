using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Entities.Entity;
using Microsoft.AspNetCore.Identity;
using Services.Dtos;

namespace Services.Contracts
{
    public interface IUserService
    {
        Task<bool> IsExistByPhoneNumberAsync(string phoneNumber, CancellationToken cancellationToken);

        Task UpdateAsync(User user);
        
        Task UpdateSecurityStampAsync(User user);

        Task<User> CreateAsync(CreateUserDto request, CancellationToken cancellationToken);

        Task<User> FindByNameAsync(string userName);

        Task<User> FindByIdAsync(string userId);

        Task<IList<string>> GetRolesAsync(User user);

        Task AddToRoleAsync(User user,string roleName);

        Task<bool> CheckPasswordAsync(User user, string password);

        Task<IdentityResult> ChangePasswordAsync(User user, string currentPassword, string newPassword);

        Task<User> GetByIdAsync(int userId, CancellationToken cancellationToken);

        Task<User> GetByPhoneNumberAsync(string phoneNumber, CancellationToken cancellationToken);

        Task UpdateLastSeenDateAsync(User user, CancellationToken cancellationToken);
    }
}