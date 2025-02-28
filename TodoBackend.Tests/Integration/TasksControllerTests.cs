using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using Todo_Backend.Controllers;
using Todo_Backend.Data;
using Todo_Backend.Models;
using Todo_Backend.Repositories;
using Todo_Backend.Services;
using Xunit;

namespace TodoBackend.Tests.Integration
{
    public class TasksControllerTests : IDisposable
    {
        private readonly AppDbContext _context;
        private readonly TasksController _controller;

        public TasksControllerTests()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "TodoTestDb")
                .Options;

            _context = new AppDbContext(options);
            _context.Database.EnsureDeleted(); 
            _context.Database.EnsureCreated();

            var repository = new TaskRepository(_context);
            var service = new TaskService(repository);
            _controller = new TasksController(service);
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted(); 
            _context.Dispose();
        }

        [Fact]
        public async Task GetLatestTasks_ReturnsEmptyList_WhenNoTasksExist()
        {
            // Act
            var result = await _controller.GetTasks();

            // Assert
            var actionResult = Assert.IsType<ActionResult<IEnumerable<TaskItem>>>(result);
            var tasks = Assert.IsType<List<TaskItem>>(actionResult.Value);
            Assert.Empty(tasks);  
        }

        [Fact]
        public async Task CreateTask_ReturnsCreatedTask()
        {
            // Arrange
            var newTask = new TaskItem { Title = "New Task", Description = "Test Description", Completed = false };

            // Act
            var result = await _controller.CreateTask(newTask);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var createdTask = Assert.IsType<TaskItem>(createdResult.Value);
            Assert.Equal("New Task", createdTask.Title);
        }

        [Fact]
        public async Task CompleteTask_ReturnsNoContent_WhenTaskExists()
        {
            // Arrange
            var task = new TaskItem { Title = "Task to Complete", Description = "Test", Completed = false };
            _context.Tasks.Add(task);
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.CompleteTask(task.Id);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task CompleteTask_ReturnsNotFound_WhenTaskDoesNotExist()
        {
            // Act
            var result = await _controller.CompleteTask(999); 

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
    }
}
