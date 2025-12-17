using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagementSystem.Core.Entities;
using TaskManagementSystem.Core.Services.Commands;
using TaskManagementSystem.Core.Services.Queries;

namespace TaskManagementSystem.API.Controllers
{
    [Authorize(Roles = "Admin,Manager")]
    [Route("api/[controller]")]
    [ApiController]
    public class TeamsController : ControllerBase
    {
        private readonly ITeamCommandService _teamCommandService;
        private readonly ITeamQueryService _teamQueryService;

        public TeamsController(
            ITeamCommandService teamCommandService,
            ITeamQueryService teamQueryService)
        {
            _teamCommandService = teamCommandService;
            _teamQueryService = teamQueryService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllTeams()
        {
            try
            {
                var teams = await _teamQueryService.GetAllTeamsAsync();
                return Ok(teams);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTeamById(int id)
        {
            try
            {
                var team = await _teamQueryService.GetTeamByIdAsync(id);
                if (team == null)
                    return NotFound();

                return Ok(team);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateTeam([FromBody] Team team)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                team.CreatedByUserId = userId;

                var createdTeam = await _teamCommandService.CreateTeamAsync(team, userId);
                return CreatedAtAction(nameof(GetTeamById), new { id = createdTeam.Id }, createdTeam);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error creating team: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTeam(int id, [FromBody] Team team)
        {
            try
            {
                if (id != team.Id)
                    return BadRequest("Team ID mismatch");

                await _teamCommandService.UpdateTeamAsync(team);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest($"Error updating team: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTeam(int id)
        {
            try
            {
                await _teamCommandService.DeleteTeamAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest($"Error deleting team: {ex.Message}");
            }
        }

        [HttpGet("my-teams")]
        public async Task<IActionResult> GetMyTeams()
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                var teams = await _teamQueryService.GetTeamsByUserIdAsync(userId);
                return Ok(teams);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}