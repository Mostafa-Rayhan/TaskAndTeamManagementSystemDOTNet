using System;
using System.Threading.Tasks;
using TaskManagementSystem.Core.Entities;
using TaskManagementSystem.Core.Services.Commands;
using TaskManagementSystem.Infrastructure.Repositories;

namespace TaskManagementSystem.Infrastructure.Services
{
    public class TeamCommandService : ITeamCommandService
    {
        private readonly ITeamRepository _teamRepository;
        private readonly IUserRepository _userRepository;

        public TeamCommandService(ITeamRepository teamRepository, IUserRepository userRepository)
        {
            _teamRepository = teamRepository;
            _userRepository = userRepository;
        }

        public async Task<Team> CreateTeamAsync(Team team, int createdByUserId)
        {
            // Validate that creator exists
            var creator = await _userRepository.GetByIdAsync(createdByUserId);
            if (creator == null)
                throw new Exception("Creator user not found");

            team.CreatedByUserId = createdByUserId;
            team.CreatedAt = DateTime.UtcNow;
            team.IsActive = true;

            return await _teamRepository.AddAsync(team);
        }

        public async Task UpdateTeamAsync(Team team)
        {
            var existingTeam = await _teamRepository.GetByIdAsync(team.Id);
            if (existingTeam == null)
                throw new Exception("Team not found");

            // Preserve creation data
            team.CreatedAt = existingTeam.CreatedAt;
            team.CreatedByUserId = existingTeam.CreatedByUserId;
            team.IsActive = existingTeam.IsActive;

            await _teamRepository.UpdateAsync(team);
        }

        public async Task DeleteTeamAsync(int id)
        {
            var team = await _teamRepository.GetByIdAsync(id);
            if (team == null)
                throw new Exception("Team not found");

            await _teamRepository.DeleteAsync(id);
        }
    }
}