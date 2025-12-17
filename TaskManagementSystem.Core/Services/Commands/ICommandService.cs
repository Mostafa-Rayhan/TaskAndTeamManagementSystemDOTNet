using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagementSystem.Core.Entities;

namespace TaskManagementSystem.Core.Services.Commands
{
    public interface IUserCommandService
    {
        Task<User> CreateUserAsync(User user, string password);
        Task UpdateUserAsync(User user);
        Task DeleteUserAsync(int id);
    }

    public interface ITeamCommandService
    {
        Task<Team> CreateTeamAsync(Team team, int createdByUserId);
        Task UpdateTeamAsync(Team team);
        Task DeleteTeamAsync(int id);
    }

    public interface ITaskCommandService
    {
        Task<Task> CreateTaskAsync(Task task);
        Task UpdateTaskAsync(Task task);
        Task DeleteTaskAsync(int id);
        Task UpdateTaskStatusAsync(int taskId, TaskStatus status, int userId);
    }
}
