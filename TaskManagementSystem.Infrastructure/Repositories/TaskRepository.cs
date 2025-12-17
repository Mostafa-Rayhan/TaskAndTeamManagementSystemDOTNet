using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using TaskManagementSystem.Core.Entities;
using TaskManagementSystem.Core.Models;
using TaskManagementSystem.Infrastructure.Data;
using TaskManagementSystem.Infrastructure.Interfaces;

namespace TaskManagementSystem.Infrastructure.Repositories
{
    public interface ITaskRepository : IRepository<Core.Entities.Task>
    {
        Task<IEnumerable<Core.Entities.Task>> SearchTasksAsync(TaskFilterParams filterParams);
        Task<IEnumerable<Core.Entities.Task>> GetTasksByUserIdAsync(int userId);
        Task<IEnumerable<Core.Entities.Task>> GetTasksByTeamIdAsync(int teamId);
        Task UpdateTaskStatusAsync(int taskId, TaskStatus status);
    }

    public class TaskRepository : ITaskRepository
    {
        private readonly string _connectionString;

        public TaskRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<Core.Entities.Task> GetByIdAsync(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var parameters = new { Id = id };
                return await connection.QueryFirstOrDefaultAsync<Core.Entities.Task>(
                    "sp_GetTaskById",
                    parameters,
                    commandType: CommandType.StoredProcedure);
            }
        }

        public async Task<IEnumerable<Core.Entities.Task>> GetAllAsync()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                return await connection.QueryAsync<Core.Entities.Task>(
                    "sp_GetAllTasks",
                    commandType: CommandType.StoredProcedure);
            }
        }

        public async Task<Core.Entities.Task> AddAsync(Core.Entities.Task task)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var parameters = new DynamicParameters();
                parameters.Add("@Title", task.Title);
                parameters.Add("@Description", task.Description);
                parameters.Add("@Status", task.Status.ToString());
                parameters.Add("@AssignedToUserId", task.AssignedToUserId);
                parameters.Add("@CreatedByUserId", task.CreatedByUserId);
                parameters.Add("@TeamId", task.TeamId);
                parameters.Add("@DueDate", task.DueDate);
                parameters.Add("@CreatedAt", task.CreatedAt);
                parameters.Add("@Id", dbType: DbType.Int32, direction: ParameterDirection.Output);

                await connection.ExecuteAsync(
                    "sp_CreateTask",
                    parameters,
                    commandType: CommandType.StoredProcedure);

                task.Id = parameters.Get<int>("@Id");
                return task;
            }
        }

        public async Task UpdateAsync(Core.Entities.Task task)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var parameters = new
                {
                    Id = task.Id,
                    Title = task.Title,
                    Description = task.Description,
                    Status = task.Status.ToString(),
                    AssignedToUserId = task.AssignedToUserId,
                    TeamId = task.TeamId,
                    DueDate = task.DueDate,
                    UpdatedAt = task.UpdatedAt
                };

                await connection.ExecuteAsync(
                    "sp_UpdateTask",
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
                    "sp_DeleteTask",
                    parameters,
                    commandType: CommandType.StoredProcedure);
            }
        }

        public async Task<IEnumerable<Core.Entities.Task>> SearchTasksAsync(TaskFilterParams filterParams)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var parameters = new
                {
                    Status = filterParams.Status?.ToString(),
                    AssignedToUserId = filterParams.AssignedToUserId,
                    TeamId = filterParams.TeamId,
                    DueDateFrom = filterParams.DueDateFrom,
                    DueDateTo = filterParams.DueDateTo,
                    PageNumber = filterParams.PageNumber,
                    PageSize = filterParams.PageSize,
                    SortBy = filterParams.SortBy,
                    SortOrder = filterParams.SortOrder
                };

                return await connection.QueryAsync<Core.Entities.Task>(
                    "sp_SearchTasks",
                    parameters,
                    commandType: CommandType.StoredProcedure);
            }
        }

        public async Task<IEnumerable<Core.Entities.Task>> GetTasksByUserIdAsync(int userId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var parameters = new { UserId = userId };
                return await connection.QueryAsync<Core.Entities.Task>(
                    "sp_GetTasksByUserId",
                    parameters,
                    commandType: CommandType.StoredProcedure);
            }
        }

        public async Task<IEnumerable<Core.Entities.Task>> GetTasksByTeamIdAsync(int teamId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var parameters = new { TeamId = teamId };
                return await connection.QueryAsync<Core.Entities.Task>(
                    "sp_GetTasksByTeamId",
                    parameters,
                    commandType: CommandType.StoredProcedure);
            }
        }

        public async Task UpdateTaskStatusAsync(int taskId, TaskStatus status)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var parameters = new
                {
                    TaskId = taskId,
                    Status = status.ToString(),
                    UpdatedAt = DateTime.UtcNow
                };

                await connection.ExecuteAsync(
                    "sp_UpdateTaskStatus",
                    parameters,
                    commandType: CommandType.StoredProcedure);
            }
        }
    }
}
