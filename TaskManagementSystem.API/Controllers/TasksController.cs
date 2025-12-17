using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagementSystem.Core.Entities;
using TaskManagementSystem.Core.Models;
using TaskManagementSystem.Core.Services.Commands;
using TaskManagementSystem.Core.Services.Queries;

namespace TaskManagementSystem.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class TasksController : ControllerBase
    {
        private readonly ITaskCommandService _taskCommandService;
        private readonly ITaskQueryService _taskQueryService;
        private readonly IUserQueryService _userQueryService;

        public TasksController(
            ITaskCommandService taskCommandService,
            ITaskQueryService taskQueryService,
            IUserQueryService userQueryService)
        {
            _taskCommandService = taskCommandService;
            _taskQueryService = taskQueryService;
            _userQueryService = userQueryService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllTasks()
        {
            try
            {
                var tasks = await _taskQueryService.GetAllTasksAsync();
                return Ok(tasks);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTaskById(int id)
        {
            try
            {
                var task = await _taskQueryService.GetTaskByIdAsync(id);
                if (task == null)
                    return NotFound();

                return Ok(task);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("search")]
        public async Task<IActionResult> SearchTasks([FromBody] TaskFilterParams filterParams)
        {
            try
            {
                var tasks = await _taskQueryService.SearchTasksAsync(filterParams);
                return Ok(tasks);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> CreateTask([FromBody] Task task)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                task.CreatedByUserId = userId;

                var createdTask = await _taskCommandService.CreateTaskAsync(task);
                return CreatedAtAction(nameof(GetTaskById), new { id = createdTask.Id }, createdTask);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error creating task: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> UpdateTask(int id, [FromBody] Task task)
        {
            try
            {
                if (id != task.Id)
                    return BadRequest("Task ID mismatch");

                await _taskCommandService.UpdateTaskAsync(task);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest($"Error updating task: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> DeleteTask(int id)
        {
            try
            {
                await _taskCommandService.DeleteTaskAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest($"Error deleting task: {ex.Message}");
            }
        }

        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateTaskStatus(int id, [FromBody] TaskStatus status)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                await _taskCommandService.UpdateTaskStatusAsync(id, status, userId);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest($"Error updating task status: {ex.Message}");
            }
        }

        [HttpGet("my-tasks")]
        public async Task<IActionResult> GetMyTasks()
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                var tasks = await _taskQueryService.GetTasksByUserIdAsync(userId);
                return Ok(tasks);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}