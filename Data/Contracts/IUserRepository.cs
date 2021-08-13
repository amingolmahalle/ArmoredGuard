using System.Threading;
using System.Threading.Tasks;
using Entities.User;

namespace Data.Contracts
{
    public interface IUserRepository : IRepository<User>
    {
        Task UpdateLastLoginDateAsync(User user, CancellationToken cancellationToken);
    }
}