using Moq;
using System;
using System.Threading.Tasks;
using TaskManagementSystem.Core.Entities;
using TaskManagementSystem.Core.Services.Commands;
using TaskManagementSystem.Infrastructure.Repositories;
using Xunit;

namespace TaskManagementSystem.Tests.Services
{
    public class TaskServiceTests
    {
        private readonly Mock<ITaskRepository> _taskRepositoryMock;
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<ITeamRepository> _teamRepositoryMock;
        private readonly TaskCommandService _taskService;

        public TaskServiceTests()
        {
            _taskRepositoryMock = new Mock<ITaskRepository>();
            _userRepositoryMock = new Mock<IUserRepository>();
            _teamRepositoryMock = new Mock<ITeamRepository>();

            _taskService = new TaskCommandService(
                _taskRepositoryMock.Object,
                _userRepositoryMock.Object,
                _teamRepositoryMock.Object);
        }

        [Fact]
        public async System.Threading.Tasks.Task CreateTaskAsync_ValidTask_ReturnsCreatedTask()
        {
            // Arrange
            var task = new Core.Entities.Task
            {
                Title = "Test Task",
                Description = "Test Description",
                AssignedToUserId = 1,
                CreatedByUserId = 2,
                TeamId = 1,
                DueDate = DateTime.UtcNow.AddDays(7)
            };

            _userRepositoryMock.Setup(x => x.GetByIdAsync(1))
                .ReturnsAsync(new User { Id = 1, IsActive = true });

            _teamRepositoryMock.Setup(x => x.GetByIdAsync(1))
                .ReturnsAsync(new Team { Id = 1, IsActive = true });

            _taskRepositoryMock.Setup(x => x.AddAsync(It.IsAny<Core.Entities.Task>()))
                .ReturnsAsync(task);

            // Act
            var result = await _taskService.CreateTaskAsync(task);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Test Task", result.Title);
            Assert.Equal(TaskStatus.Todo, result.Status);
        }

        [Fact]
        public async System.Threading.Tasks.Task UpdateTaskStatusAsync_ValidUpdate_UpdatesStatus()
        {
            // Arrange
            var taskId = 1;
            var userId = 1;
            var status = TaskStatus.InProgress;

            var existingTask = new Core.Entities.Task
            {
                Id = taskId,
                AssignedToUserId = userId,
                Status = TaskStatus.Todo
            };

            _taskRepositoryMock.Setup(x => x.GetByIdAsync(taskId))
                .ReturnsAsync(existingTask);

            // Act
            await _taskService.UpdateTaskStatusAsync(taskId, status, userId);

            // Assert
            _taskRepositoryMock.Verify(x => x.UpdateTaskStatusAsync(taskId, status), Times.Once);
        }

        [Fact]
        public async System.Threading.Tasks.Task UpdateTaskStatusAsync_WrongUser_ThrowsException()
        {
            // Arrange
            var taskId = 1;
            var wrongUserId = 2;
            var status = TaskStatus.InProgress;

            var existingTask = new Core.Entities.Task
            {
                Id = taskId,
                AssignedToUserId = 1, // Different user
                Status = TaskStatus.Todo
            };

            _taskRepositoryMock.Setup(x => x.GetByIdAsync(taskId))
                .ReturnsAsync(existingTask);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(
                () => _taskService.UpdateTaskStatusAsync(taskId, status, wrongUserId));
        }
    }
}