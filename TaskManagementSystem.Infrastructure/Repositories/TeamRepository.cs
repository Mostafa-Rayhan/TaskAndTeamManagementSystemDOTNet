using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Data.SqlClient; // Add this
using Microsoft.Extensions.Configuration;
using TaskManagementSystem.Core.Entities;
using TaskManagementSystem.Infrastructure.Data;
using TaskManagementSystem.Infrastructure.Interfaces;

namespace TaskManagementSystem.Infrastructure.Repositories
{
    public interface ITeamRepository : IRepository<Team>
    {
        Task<IEnumerable<Team>> GetTeamsByUserIdAsync(int userId);
    }

    public class TeamRepository : ITeamRepository
    {
        private readonly string _connectionString;
        private readonly DatabaseContext _context;

        public TeamRepository(IConfiguration configuration, DatabaseContext context)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            _context = context;
        }

        public async Task<Team> GetByIdAsync(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var parameters = new { Id = id };
                return await connection.QueryFirstOrDefaultAsync<Team>(
                    "sp_GetTeamById",
                    parameters,
                    commandType: CommandType.StoredProcedure);
            }
        }

        public async Task<IEnumerable<Team>> GetAllAsync()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                return await connection.QueryAsync<Team>(
                    "sp_GetAllTeams",
                    commandType: CommandType.StoredProcedure);
            }
        }

        public async Task<Team> AddAsync(Team team)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var parameters = new DynamicParameters();
                parameters.Add("@Name", team.Name);
                parameters.Add("@Description", team.Description);
                parameters.Add("@CreatedByUserId", team.CreatedByUserId);
                parameters.Add("@CreatedAt", team.CreatedAt);
                parameters.Add("@IsActive", team.IsActive);
                parameters.Add("@Id", dbType: DbType.Int32, direction: ParameterDirection.Output);

                await connection.ExecuteAsync(
                    "sp_CreateTeam",
                    parameters,
                    commandType: CommandType.StoredProcedure);

                team.Id = parameters.Get<int>("@Id");
                return team;
            }
        }

        public async Task UpdateAsync(Team team)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var parameters = new
                {
                    Id = team.Id,
                    Name = team.Name,
                    Description = team.Description,
                    IsActive = team.IsActive
                };

                await connection.ExecuteAsync(
                    "sp_UpdateTeam",
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
                    "sp_DeleteTeam",
                    parameters,
                    commandType: CommandType.StoredProcedure);
            }
        }

        public async Task<IEnumerable<Team>> GetTeamsByUserIdAsync(int userId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var parameters = new { UserId = userId };
                return await connection.QueryAsync<Team>(
                    "sp_GetTeamsByUserId",
                    parameters,
                    commandType: CommandType.StoredProcedure);
            }
        }
    }
}