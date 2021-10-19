#nullable enable
using System;
using System.Threading;
using System.Threading.Tasks;
using Data.Contracts;
using Entities.Entity;
using Microsoft.EntityFrameworkCore;

namespace Data.Repositories
{
    public class UserRepository : Repository<User, int>, IUserRepository
    {
        public UserRepository(ApplicationDbContext dbContext)
            : base(dbContext)
        {
        }

        public async Task UpdateLastSeenDateAsync(User user, CancellationToken cancellationToken)
        {
            user.LastSeenDate = DateTimeOffset.Now;

            await UpdateAsync(user, cancellationToken);
        }

        public Task<bool> IsExistUserByPhoneNumberAsync(string phoneNumber, CancellationToken cancellationToken)
        {
            //TODO : PhoneNumber must be verified (PhoneNumberConfirmed == true)
            return TableNoTracking.AnyAsync(u => u.PhoneNumber == phoneNumber, cancellationToken);
        }

        public Task<User> GetUserByPhoneNumberAsync(string phoneNumber, CancellationToken cancellationToken)
        {
            return TableNoTracking.SingleOrDefaultAsync(u => u.PhoneNumber == phoneNumber, cancellationToken);
        }
        
        public Task<bool> IsExistUserByEmailAsync(string email, CancellationToken cancellationToken)
        {
            return TableNoTracking.AnyAsync(u => u.Email == email, cancellationToken);
        }
        
        public Task<bool> IsExistUserByUsernameAsync(string username, CancellationToken cancellationToken)
        {
            return TableNoTracking.AnyAsync(u => u.UserName == username, cancellationToken);
        }
    }
}