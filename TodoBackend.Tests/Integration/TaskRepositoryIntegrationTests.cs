using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Todo_Backend.Data;
using Todo_Backend.Models;
using Todo_Backend.Repositories;
using Xunit;

namespace TodoBackend.Tests.Integration
{
    public class TaskRepositoryIntegrationTests : IDisposable
    {
        private readonly AppDbContext _context;
        private readonly TaskRepository _repository;

        public TaskRepositoryIntegrationTests()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) 
                .Options;

            _context = new AppDbContext(options);
            _context.Database.EnsureCreated(); 
            _repository = new TaskRepository(_context);
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted(); 
            _context.Dispose();
        }

        [Fact]
        public async Task CreateTaskAsync_AddsTaskToDatabase()
        {
            // Arrange
            var task = new TaskItem { Title = "Task 1", Description = "Description 1", Completed = false };

            // Act
            var createdTask = await _repository.CreateTaskAsync(task);
            var savedTask = await _context.Tasks.FindAsync(createdTask.Id);

            // Assert
            Assert.NotNull(savedTask);
            Assert.Equal("Task 1", savedTask.Title);
        }

        [Fact]
        public async Task GetLatestTasksAsync_ReturnsUncompletedTasksOrderedByCreatedAt()
        {
            // Arrange
            var task1 = new TaskItem { Title = "Task 1", Description = "Task 1 Description", Completed = false, CreatedAt = DateTime.UtcNow };
            var task2 = new TaskItem { Title = "Task 2", Description = "Task 2 Description", Completed = false, CreatedAt = DateTime.UtcNow.AddHours(-1) };
            var task3 = new TaskItem { Title = "Task 3", Description = "Task 3 Description", Completed = true }; 

            _context.Tasks.AddRange(task1, task2, task3);
            await _context.SaveChangesAsync();

            // Act
            var tasks = await _repository.GetLatestTasksAsync();

            // Assert
            Assert.Equal(2, tasks.Count); 
            Assert.Equal("Task 1", tasks[0].Title); 
            Assert.Equal("Task 2", tasks[1].Title); 
        }


        [Fact]
        public async Task GetTaskByIdAsync_ReturnsTask_WhenExists()
        {
            // Arrange
            var task = new TaskItem { Title = "Existing Task", Description = "Description" };
            _context.Tasks.Add(task);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetTaskByIdAsync(task.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Existing Task", result.Title);
        }

        [Fact]
        public async Task GetTaskByIdAsync_ReturnsNull_WhenTaskDoesNotExist()
        {
            // Act
            var result = await _repository.GetTaskByIdAsync(999); 

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task MarkTaskCompletedAsync_SetsTaskAsCompleted_WhenExists()
        {
            // Arrange
            var task = new TaskItem { Title = "Task to Complete", Description = "Fixing test error", Completed = false }; 
            _context.Tasks.Add(task);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.MarkTaskCompletedAsync(task.Id);
            var updatedTask = await _context.Tasks.FindAsync(task.Id);

            // Assert
            Assert.True(result);
            Assert.NotNull(updatedTask);
            Assert.True(updatedTask.Completed);
        }


        [Fact]
        public async Task MarkTaskCompletedAsync_ReturnsFalse_WhenTaskDoesNotExist()
        {
            // Act
            var result = await _repository.MarkTaskCompletedAsync(999); 

            // Assert
            Assert.False(result);
        }
    }
}
