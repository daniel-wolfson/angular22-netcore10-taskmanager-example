using TaskManager.Api.Models;
using TaskManager.Api.Requests;
using TaskManager.Api.Services.Results;

namespace TaskManager.Api.Services
{
    public interface ITaskService
    {
        IReadOnlyList<TaskItem> GetTasks(string? title);
        TaskItem CreateTask(CreateTaskRequest request);
        MarkAsDoneResult MarkAsDone(Guid id);
        TaskItem? GetTaskById(Guid id);
    }
}