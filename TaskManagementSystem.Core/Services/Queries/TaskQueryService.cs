using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagementSystem.Core.Entities;
using TaskManagementSystem.Core.Models;
using TaskManagementSystem.Infrastructure.Repositories;

namespace TaskManagementSystem.Core.Services.Queries
{
    public class TaskQueryService : ITaskQueryService
    {
        private readonly ITaskRepository _taskRepository;

        public TaskQueryService(ITaskRepository taskRepository)
        {
            _taskRepository = taskRepository;
        }

        public async Task<Task> GetTaskByIdAsync(int id)
        {
            return await _taskRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<Task>> GetAllTasksAsync()
        {
            return await _taskRepository.GetAllAsync();
        }

        public async Task<IEnumerable<Task>> SearchTasksAsync(TaskFilterParams filterParams)
        {
            return await _taskRepository.SearchTasksAsync(filterParams);
        }

        public async Task<IEnumerable<Task>> GetTasksByUserIdAsync(int userId)
        {
            return await _taskRepository.GetTasksByUserIdAsync(userId);
        }

        public async Task<IEnumerable<Task>> GetTasksByTeamIdAsync(int teamId)
        {
            return await _taskRepository.GetTasksByTeamIdAsync(teamId);
        }
    }
}
