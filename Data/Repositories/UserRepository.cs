using System;
using System.Threading;
using System.Threading.Tasks;
using Data.Contracts;
using Entities.User;

namespace Data.Repositories
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(ApplicationDbContext dbContext)
            : base(dbContext)
        {
        }

        public async Task UpdateLastLoginDateAsync(User user, CancellationToken cancellationToken)
        {
            user.LastLoginDate = DateTimeOffset.Now;

            await UpdateAsync(user, cancellationToken);
        }

        public async Task UpdateLastSeenDateAsync(User user, CancellationToken cancellationToken)
        {
            user.LastSeenDate = DateTimeOffset.Now;

            await UpdateAsync(user, cancellationToken);
        }
    }
}