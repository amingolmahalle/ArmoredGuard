using System.Threading;
using System.Threading.Tasks;
using Entities.User;

namespace Services.Contracts
{
    public interface IUserService
    {
        Task<bool> IsExistUserByPhoneNumberAsync(string phoneNumber, CancellationToken cancellationToken);
        Task<User> GetByIdAsync(int userId, CancellationToken cancellationToken);
        Task UpdateLastSeenDateAsync(User user, CancellationToken cancellationToken);
    }
}