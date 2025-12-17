//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//using System.Threading.Tasks;
//using TaskManagementSystem.Core.Entities;
//using TaskManagementSystem.Core.Models;

//namespace TaskManagementSystem.Core.Services.Queries
//{
//    public interface IUserQueryService
//    {
//        Task<User> GetUserByIdAsync(int id);
//        Task<IEnumerable<User>> GetAllUsersAsync();
//        Task<User> GetUserByEmailAsync(string email);
//        Task<IEnumerable<User>> GetUsersByRoleAsync(UserRole role);
//    }

//    public interface ITeamQueryService
//    {
//        Task<Team> GetTeamByIdAsync(int id);
//        Task<IEnumerable<Team>> GetAllTeamsAsync();
//        Task<IEnumerable<Team>> GetTeamsByUserIdAsync(int userId);
//    }

//    public interface ITaskQueryService
//    {
//        Task<Task> GetTaskByIdAsync(int id);
//        Task<IEnumerable<Task>> GetAllTasksAsync();
//        Task<IEnumerable<Task>> SearchTasksAsync(TaskFilterParams filterParams);
//        Task<IEnumerable<Task>> GetTasksByUserIdAsync(int userId);
//        Task<IEnumerable<Task>> GetTasksByTeamIdAsync(int teamId);
//    }
//}