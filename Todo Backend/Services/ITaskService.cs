using Todo_Backend.Models;

namespace Todo_Backend.Services
{
    public interface ITaskService
    {
        Task<List<TaskItem>> GetLatestTasksAsync();
        Task<TaskItem> CreateTaskAsync(TaskItem task);
        Task<bool> MarkTaskCompletedAsync(int id);
    }
}
