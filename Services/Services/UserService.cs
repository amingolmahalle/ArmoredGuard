using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Common.Enums;
using Common.Helpers;
using Data.Contracts;
using Entities.Entity;
using Microsoft.AspNetCore.Identity;
using Services.Contracts;
using Services.Contracts.Redis;
using Services.Dtos;

namespace Services.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        private readonly UserManager<User> _userManager;

        private readonly IRedisService _redisService;

        public UserService(
            IUserRepository userRepository,
            UserManager<User> userManager,
            IRedisService redisService)
        {
            _userRepository = userRepository;
            _userManager = userManager;
            _redisService = redisService;
        }

        public Task<bool> IsExistByPhoneNumberAsync(string phoneNumber, CancellationToken cancellationToken)
        {
            return _userRepository.IsExistUserByPhoneNumberAsync(phoneNumber, cancellationToken);
        }

        public async Task UpdateAsync(User user)
        {
            await _userManager.UpdateAsync(user);
        }

        public async Task UpdateSecurityStampAsync(User user)
        {
            await _userManager.UpdateSecurityStampAsync(user);
        }

        public async Task<User> CreateAsync(CreateUserDto request, CancellationToken cancellationToken)
        {
            switch (request.RegisterType)
            {
                case RegisterType.Password:
                    if (string.IsNullOrEmpty(request.Password?.Trim()))
                        throw new InvalidDataException("Password should not be empty");
                    break;
                case RegisterType.Otp:
                    if (string.IsNullOrEmpty(request.OtpCode?.Trim()))
                        throw new InvalidDataException("OtpCode should not be empty");

                    string otpCode = await _redisService.GetAsync<string>(request.PhoneNumber, cancellationToken);

                    if (otpCode != request.OtpCode)
                        throw new Exception($" otp code {request.OtpCode} for this phone number is invalid");

                    if (string.IsNullOrEmpty(request.Password?.Trim()))
                        request.Password = Randomizer.GeneratePassword();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            var user = new User
            {
                BirthDate = request.BirthDate,
                FullName = request.FullName,
                Gender = request.Gender,
                UserName = request.UserName,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
                IsActive = true
            };

            await _userManager.CreateAsync(user, request.Password);

            if (request.RegisterType == RegisterType.Otp)
                await _redisService.RemoveAsync(request.PhoneNumber, cancellationToken);

            return user;
        }

        public Task<User> FindByNameAsync(string userName)
        {
            return _userManager.FindByNameAsync(userName);
        }

        public Task<User> FindByIdAsync(string userId)
        {
            return _userManager.FindByIdAsync(userId);
        }

        public Task<IList<string>> GetRolesAsync(User user)
        {
            return _userManager.GetRolesAsync(user);
        }

        public async Task AddToRoleAsync(User user, string roleName)
        {
            await _userManager.AddToRoleAsync(user, roleName);
        }

        public Task<bool> CheckPasswordAsync(User user, string password)
        {
            return _userManager.CheckPasswordAsync(user, password);
        }

        public Task<IdentityResult> ChangePasswordAsync(User user, string currentPassword, string newPassword)
        {
            return _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
        }

        public Task<User> GetByPhoneNumberAsync(string phoneNumber, CancellationToken cancellationToken)
        {
            return _userRepository.GetUserByPhoneNumberAsync(phoneNumber, cancellationToken);
        }

        public Task UpdateLastSeenDateAsync(User user, CancellationToken cancellationToken)
        {
            return _userRepository.UpdateLastSeenDateAsync(user, cancellationToken);
        }

        public Task<bool> IsExistUserByEmailAsync(string email, CancellationToken cancellationToken)
        {
            return _userRepository.IsExistUserByEmailAsync(email, cancellationToken);
        }

        public Task<bool> IsExistUserByUsernameAsync(string username, CancellationToken cancellationToken)
        {
            return _userRepository.IsExistUserByUsernameAsync(username, cancellationToken);
        }
    }
}