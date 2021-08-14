using System.Threading;
using System.Threading.Tasks;
using Entities.User;

namespace Data.Contracts
{
    public interface IUserRepository : IRepository<User>
    {
        Task UpdateLastSeenDateAsync(User user, CancellationToken cancellationToken);
    }
}