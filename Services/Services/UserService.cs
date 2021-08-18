using System.Threading;
using System.Threading.Tasks;
using Data.Contracts;
using Entities.User;
using Services.Contracts;

namespace Services.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public Task<bool> IsExistUserByPhoneNumberAsync(string phoneNumber, CancellationToken cancellationToken)
        {
            return _userRepository.GetUserByPhoneNumberAsync(phoneNumber, cancellationToken);
        }

        public async Task<User> GetByIdAsync(int userId, CancellationToken cancellationToken)
        {
            return await _userRepository.GetByIdAsync(cancellationToken, userId);
        }

        public Task UpdateLastSeenDateAsync(User user, CancellationToken cancellationToken)
        {
            return _userRepository.UpdateLastSeenDateAsync(user, cancellationToken);
        }
    }
}