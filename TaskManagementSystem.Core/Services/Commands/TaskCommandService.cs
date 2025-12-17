using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagementSystem.Core.Entities;
using TaskManagementSystem.Infrastructure.Repositories;

namespace TaskManagementSystem.Core.Services.Commands
{
    public class TaskCommandService : ITaskCommandService
    {
        private readonly ITaskRepository _taskRepository;
        private readonly IUserRepository _userRepository;
        private readonly ITeamRepository _teamRepository;

        public TaskCommandService(
            ITaskRepository taskRepository,
            IUserRepository userRepository,
            ITeamRepository teamRepository)
        {
            _taskRepository = taskRepository;
            _userRepository = userRepository;
            _teamRepository = teamRepository;
        }

        public async Task<Task> CreateTaskAsync(Task task)
        {
            // Validate assigned user exists
            if (task.AssignedToUserId.HasValue)
            {
                var assignedUser = await _userRepository.GetByIdAsync(task.AssignedToUserId.Value);
                if (assignedUser == null)
                    throw new Exception("Assigned user not found");
            }

            // Validate team exists
            if (task.TeamId.HasValue)
            {
                var team = await _teamRepository.GetByIdAsync(task.TeamId.Value);
                if (team == null)
                    throw new Exception("Team not found");
            }

            task.CreatedAt = DateTime.UtcNow;
            task.Status = TaskStatus.Todo;

            return await _taskRepository.AddAsync(task);
        }

        public async Task UpdateTaskAsync(Task task)
        {
            var existingTask = await _taskRepository.GetByIdAsync(task.Id);
            if (existingTask == null)
                throw new Exception("Task not found");

            // Validate assigned user exists
            if (task.AssignedToUserId.HasValue)
            {
                var assignedUser = await _userRepository.GetByIdAsync(task.AssignedToUserId.Value);
                if (assignedUser == null)
                    throw new Exception("Assigned user not found");
            }

            // Validate team exists
            if (task.TeamId.HasValue)
            {
                var team = await _teamRepository.GetByIdAsync(task.TeamId.Value);
                if (team == null)
                    throw new Exception("Team not found");
            }

            task.UpdatedAt = DateTime.UtcNow;
            task.CreatedAt = existingTask.CreatedAt;
            task.CreatedByUserId = existingTask.CreatedByUserId;

            await _taskRepository.UpdateAsync(task);
        }

        public async Task DeleteTaskAsync(int id)
        {
            var task = await _taskRepository.GetByIdAsync(id);
            if (task == null)
                throw new Exception("Task not found");

            await _taskRepository.DeleteAsync(id);
        }

        public async Task UpdateTaskStatusAsync(int taskId, TaskStatus status, int userId)
        {
            var task = await _taskRepository.GetByIdAsync(taskId);
            if (task == null)
                throw new Exception("Task not found");

            // Check if user is assigned to this task
            if (task.AssignedToUserId != userId)
                throw new Exception("You are not assigned to this task");

            await _taskRepository.UpdateTaskStatusAsync(taskId, status);
        }
    }
}
