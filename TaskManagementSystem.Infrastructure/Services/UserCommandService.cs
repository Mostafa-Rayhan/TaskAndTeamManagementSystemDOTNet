using System;
using System.Threading.Tasks;
using TaskManagementSystem.Core.Entities;
using TaskManagementSystem.Core.Services.Commands;
using TaskManagementSystem.Infrastructure.Data;
using TaskManagementSystem.Infrastructure.Repositories;

namespace TaskManagementSystem.Infrastructure.Services
{
    public class UserCommandService : IUserCommandService
    {
        private readonly IUserRepository _userRepository;
        private readonly PasswordHasher _passwordHasher;

        public UserCommandService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
            _passwordHasher = new PasswordHasher();
        }

        public async Task<User> CreateUserAsync(User user, string password)
        {
            // Check if user with same email exists
            var existingUser = await _userRepository.GetByEmailAsync(user.Email);
            if (existingUser != null)
                throw new Exception("User with this email already exists");

            user.PasswordHash = _passwordHasher.HashPassword(password);
            user.CreatedAt = DateTime.UtcNow;
            user.IsActive = true;

            return await _userRepository.AddAsync(user);
        }

        public async Task UpdateUserAsync(User user)
        {
            var existingUser = await _userRepository.GetByIdAsync(user.Id);
            if (existingUser == null)
                throw new Exception("User not found");

            // Don't update password through this method
            user.PasswordHash = existingUser.PasswordHash;
            user.CreatedAt = existingUser.CreatedAt;
            await _userRepository.UpdateAsync(user);
        }

        public async Task DeleteUserAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
                throw new Exception("User not found");

            await _userRepository.DeleteAsync(id);
        }
    }
}