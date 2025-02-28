using System.Collections.Generic;
using System.Threading.Tasks;
using Todo_Backend.Models;

namespace Todo_Backend.Repositories
{
    public interface ITaskRepository
    {
        Task<List<TaskItem>> GetLatestTasksAsync();
        Task<TaskItem> GetTaskByIdAsync(int id);
        Task<TaskItem> CreateTaskAsync(TaskItem task);
        Task<bool> MarkTaskCompletedAsync(int id);
    }
}
