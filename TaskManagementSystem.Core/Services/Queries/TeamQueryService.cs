using System.Collections.Generic;
using System.Threading.Tasks;
using TaskManagementSystem.Core.Entities;
using TaskManagementSystem.Infrastructure.Repositories;

namespace TaskManagementSystem.Core.Services.Queries
{
    public interface ITeamQueryService
    {
        Task<Team> GetTeamByIdAsync(int id);
        Task<IEnumerable<Team>> GetAllTeamsAsync();
        Task<IEnumerable<Team>> GetTeamsByUserIdAsync(int userId);
    }

    public class TeamQueryService : ITeamQueryService
    {
        private readonly ITeamRepository _teamRepository;

        public TeamQueryService(ITeamRepository teamRepository)
        {
            _teamRepository = teamRepository;
        }

        public async Task<Team> GetTeamByIdAsync(int id)
        {
            return await _teamRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<Team>> GetAllTeamsAsync()
        {
            return await _teamRepository.GetAllAsync();
        }

        public async Task<IEnumerable<Team>> GetTeamsByUserIdAsync(int userId)
        {
            return await _teamRepository.GetTeamsByUserIdAsync(userId);
        }
    }
}