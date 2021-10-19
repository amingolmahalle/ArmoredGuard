using System.Threading;
using System.Threading.Tasks;
using Entities.Entity;

namespace Data.Contracts
{
    public interface IUserRepository : IRepository<User, int>
    {
        Task UpdateLastSeenDateAsync(User user, CancellationToken cancellationToken);

        Task<bool> IsExistUserByPhoneNumberAsync(string phoneNumber, CancellationToken cancellationToken);

        Task<User> GetUserByPhoneNumberAsync(string phoneNumber, CancellationToken cancellationToken);
        
        Task<bool> IsExistUserByEmailAsync(string email, CancellationToken cancellationToken);

        Task<bool> IsExistUserByUsernameAsync(string username, CancellationToken cancellationToken);
    }
}