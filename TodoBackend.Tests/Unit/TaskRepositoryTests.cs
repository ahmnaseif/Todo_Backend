using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Todo_Backend.Repositories;
using Todo_Backend.Data;
using Todo_Backend.Models;
using Xunit;

namespace TodoBackend.Tests.Unit
{
    public class TaskRepositoryTests : IDisposable
    {
        private readonly AppDbContext _context;
        private readonly TaskRepository _repository;

        public TaskRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Unique DB per test
                .Options;

            _context = new AppDbContext(options);
            _repository = new TaskRepository(_context);
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Fact]
        public async Task GetLatestTasksAsync_ReturnsLatestTasks()
        {
            // Arrange
            await _context.Tasks.AddRangeAsync(
                new TaskItem { Id = 1, Title = "Task 1", Description = "Description 1", Completed = false, CreatedAt = DateTime.UtcNow },
                new TaskItem { Id = 2, Title = "Task 2", Description = "Description 2", Completed = false, CreatedAt = DateTime.UtcNow.AddHours(-1) }
            );
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetLatestTasksAsync();

            // Assert
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task GetTaskByIdAsync_ReturnsTask_WhenTaskExists()
        {
            // Arrange
            var task = new TaskItem { Id = 1, Title = "Test Task", Description = "Description", Completed = false };
            await _context.Tasks.AddAsync(task);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetTaskByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Test Task", result.Title);
        }

        [Fact]
        public async Task GetTaskByIdAsync_ReturnsNull_WhenTaskDoesNotExist()
        {
            // Act
            var result = await _repository.GetTaskByIdAsync(999); // Non-existent ID

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task CreateTaskAsync_AddsTaskToDatabase()
        {
            // Arrange
            var newTask = new TaskItem { Title = "New Task", Description = "Description", Completed = false };

            // Act
            var createdTask = await _repository.CreateTaskAsync(newTask);
            var taskInDb = await _context.Tasks.FindAsync(createdTask.Id);

            // Assert
            Assert.NotNull(taskInDb);
            Assert.Equal("New Task", taskInDb.Title);
        }

        [Fact]
        public async Task MarkTaskCompletedAsync_MarksTaskAsCompleted_WhenTaskExists()
        {
            // Arrange
            var task = new TaskItem { Id = 1, Title = "Task to Complete", Description = "Description", Completed = false };
            await _context.Tasks.AddAsync(task);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.MarkTaskCompletedAsync(1);
            var updatedTask = await _context.Tasks.FindAsync(1);

            // Assert
            Assert.True(result);
            Assert.NotNull(updatedTask);
            Assert.True(updatedTask.Completed);
        }

        [Fact]
        public async Task MarkTaskCompletedAsync_ReturnsFalse_WhenTaskDoesNotExist()
        {
            // Act
            var result = await _repository.MarkTaskCompletedAsync(999); // Non-existent task

            // Assert
            Assert.False(result);
        }
    }
}
