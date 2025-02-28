using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Todo_Backend.Models;
using Todo_Backend.Repositories;
using Todo_Backend.Services;
using Xunit;

namespace TodoBackend.Tests.Unit
{
    public class TaskServiceTests
    {
        private readonly Mock<ITaskRepository> _mockRepository;
        private readonly TaskService _service;

        public TaskServiceTests()
        {
            _mockRepository = new Mock<ITaskRepository>();
            _service = new TaskService(_mockRepository.Object);
        }

        [Fact]
        public async Task GetLatestTasksAsync_ReturnsTasks()
        {
            // Arrange
            var tasks = new List<TaskItem>
            {
                new TaskItem { Id = 1, Title = "Task 1", Description = "Description 1", Completed = false },
                new TaskItem { Id = 2, Title = "Task 2", Description = "Description 2", Completed = false }
            };

            _mockRepository.Setup(repo => repo.GetLatestTasksAsync()).ReturnsAsync(tasks);

            // Act
            var result = await _service.GetLatestTasksAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task CreateTaskAsync_ReturnsCreatedTask()
        {
            // Arrange
            var task = new TaskItem { Id = 1, Title = "Task 1", Description = "Description 1", Completed = false };
            _mockRepository.Setup(repo => repo.CreateTaskAsync(task)).ReturnsAsync(task);

            // Act
            var result = await _service.CreateTaskAsync(task);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(task.Id, result.Id);
            Assert.Equal("Task 1", result.Title);
        }

        [Fact]
        public async Task MarkTaskCompletedAsync_ReturnsTrue_WhenTaskExists()
        {
            // Arrange
            _mockRepository.Setup(repo => repo.MarkTaskCompletedAsync(1)).ReturnsAsync(true);

            // Act
            var result = await _service.MarkTaskCompletedAsync(1);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task MarkTaskCompletedAsync_ReturnsFalse_WhenTaskDoesNotExist()
        {
            // Arrange
            _mockRepository.Setup(repo => repo.MarkTaskCompletedAsync(999)).ReturnsAsync(false);

            // Act
            var result = await _service.MarkTaskCompletedAsync(999);

            // Assert
            Assert.False(result);
        }
    }
}
