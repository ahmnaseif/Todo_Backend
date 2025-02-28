using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Todo_Backend.Data;
using Todo_Backend.Models;
using Todo_Backend.Repositories;


namespace Todo_Backend.Services
{
    public class TaskService : ITaskService
    {
        private readonly ITaskRepository _taskRepository;

        public TaskService(ITaskRepository taskRepository)
        {
            _taskRepository = taskRepository;
        }

        public async Task<List<TaskItem>> GetLatestTasksAsync()
        {
            return await _taskRepository.GetLatestTasksAsync();
        }

        public async Task<TaskItem> CreateTaskAsync(TaskItem task)
        {
            return await _taskRepository.CreateTaskAsync(task);
        }

        public async Task<bool> MarkTaskCompletedAsync(int id)
        {
            return await _taskRepository.MarkTaskCompletedAsync(id);
        }
    }
}
