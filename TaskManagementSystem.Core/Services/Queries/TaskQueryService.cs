using System.Collections.Generic;
using System.Threading.Tasks;
using TaskEntity = TaskManagementSystem.Core.Entities.Task;
using TaskManagementSystem.Core.Entities;
using TaskManagementSystem.Core.Models;
using TaskManagementSystem.Infrastructure.Repositories;

namespace TaskManagementSystem.Core.Services.Queries
{
    public interface ITaskQueryService
    {
        Task<TaskEntity> GetTaskByIdAsync(int id);
        Task<IEnumerable<TaskEntity>> GetAllTasksAsync();
        Task<IEnumerable<TaskEntity>> SearchTasksAsync(TaskFilterParams filterParams);
        Task<IEnumerable<TaskEntity>> GetTasksByUserIdAsync(int userId);
        Task<IEnumerable<TaskEntity>> GetTasksByTeamIdAsync(int teamId);
    }

    public class TaskQueryService : ITaskQueryService
    {
        private readonly ITaskRepository _taskRepository;

        public TaskQueryService(ITaskRepository taskRepository)
        {
            _taskRepository = taskRepository;
        }

        public async Task<TaskEntity> GetTaskByIdAsync(int id)
        {
            return await _taskRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<TaskEntity>> GetAllTasksAsync()
        {
            return await _taskRepository.GetAllAsync();
        }

        public async Task<IEnumerable<TaskEntity>> SearchTasksAsync(TaskFilterParams filterParams)
        {
            return await _taskRepository.SearchTasksAsync(filterParams);
        }

        public async Task<IEnumerable<TaskEntity>> GetTasksByUserIdAsync(int userId)
        {
            return await _taskRepository.GetTasksByUserIdAsync(userId);
        }

        public async Task<IEnumerable<TaskEntity>> GetTasksByTeamIdAsync(int teamId)
        {
            return await _taskRepository.GetTasksByTeamIdAsync(teamId);
        }
    }
}