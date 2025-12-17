using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using TaskManagementSystem.Core.Entities;
using TaskManagementSystem.Infrastructure.Data;
using TaskManagementSystem.Infrastructure.Interfaces;

namespace TaskManagementSystem.Infrastructure.Repositories
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User> GetByEmailAsync(string email);
        Task<User> GetUserWithPasswordAsync(string email);
        Task<IEnumerable<User>> GetUsersByRoleAsync(UserRole role);
    }

    public class UserRepository : IUserRepository
    {
        private readonly string _connectionString;
        private readonly DatabaseContext _context;

        public UserRepository(IConfiguration configuration, DatabaseContext context)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            _context = context;
        }

        public async Task<User> GetByIdAsync(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var parameters = new { Id = id };
                return await connection.QueryFirstOrDefaultAsync<User>(
                    "sp_GetUserById",
                    parameters,
                    commandType: CommandType.StoredProcedure);
            }
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                return await connection.QueryAsync<User>(
                    "sp_GetAllUsers",
                    commandType: CommandType.StoredProcedure);
            }
        }

        public async Task<User> AddAsync(User user)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var parameters = new DynamicParameters();
                parameters.Add("@FullName", user.FullName);
                parameters.Add("@Email", user.Email);
                parameters.Add("@PasswordHash", user.PasswordHash);
                parameters.Add("@Role", user.Role.ToString());
                parameters.Add("@CreatedAt", user.CreatedAt);
                parameters.Add("@IsActive", user.IsActive);
                parameters.Add("@Id", dbType: DbType.Int32, direction: ParameterDirection.Output);

                await connection.ExecuteAsync(
                    "sp_CreateUser",
                    parameters,
                    commandType: CommandType.StoredProcedure);

                user.Id = parameters.Get<int>("@Id");
                return user;
            }
        }

        public async Task UpdateAsync(User user)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var parameters = new
                {
                    Id = user.Id,
                    FullName = user.FullName,
                    Email = user.Email,
                    Role = user.Role.ToString(),
                    IsActive = user.IsActive
                };

                await connection.ExecuteAsync(
                    "sp_UpdateUser",
                    parameters,
                    commandType: CommandType.StoredProcedure);
            }
        }

        public async Task DeleteAsync(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var parameters = new { Id = id };
                await connection.ExecuteAsync(
                    "sp_DeleteUser",
                    parameters,
                    commandType: CommandType.StoredProcedure);
            }
        }

        public async Task<User> GetByEmailAsync(string email)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var parameters = new { Email = email };
                return await connection.QueryFirstOrDefaultAsync<User>(
                    "sp_GetUserByEmail",
                    parameters,
                    commandType: CommandType.StoredProcedure);
            }
        }

        public async Task<User> GetUserWithPasswordAsync(string email)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var parameters = new { Email = email };
                return await connection.QueryFirstOrDefaultAsync<User>(
                    "sp_GetUserWithPassword",
                    parameters,
                    commandType: CommandType.StoredProcedure);
            }
        }

        public async Task<IEnumerable<User>> GetUsersByRoleAsync(UserRole role)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var parameters = new { Role = role.ToString() };
                return await connection.QueryAsync<User>(
                    "sp_GetUsersByRole",
                    parameters,
                    commandType: CommandType.StoredProcedure);
            }
        }
    }
}