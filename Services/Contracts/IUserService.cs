using System.Threading;
using System.Threading.Tasks;
using Entities.Entity;

namespace Services.Contracts
{
    public interface IUserService
    {
        Task<bool> IsExistByPhoneNumberAsync(string phoneNumber, CancellationToken cancellationToken);
        
        Task<User> GetByIdAsync(int userId, CancellationToken cancellationToken);
        
        Task<User> GetByPhoneNumberAsync(string phoneNumber, CancellationToken cancellationToken);
        
        Task UpdateLastSeenDateAsync(User user, CancellationToken cancellationToken);
    }
}