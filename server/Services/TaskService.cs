using TaskManager.Api.Models;
using TaskManager.Api.Requests;
using TaskStatus = TaskManager.Api.Models.TaskStatus;

namespace TaskManager.Api.Services;

public class TaskService
{
    private readonly object _syncLock = new();
    private readonly List<TaskItem> _tasks = [];

    public IReadOnlyList<TaskItem> GetTasks(string? title)
    {
        lock (_syncLock)
        {
            var query = _tasks.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(title))
            {
                query = query.Where(task => task.Title.Contains(title, StringComparison.OrdinalIgnoreCase));
            }

            return query
                .OrderByDescending(task => task.CreatedDate)
                .ToList();
        }
    }

    public TaskItem CreateTask(CreateTaskRequest request)
    {
        var task = new TaskItem
        {
            Title = request.Title,
            Description = request.Description ?? string.Empty,
            CreatedDate = DateTime.UtcNow,
            Status = TaskStatus.Open
        };

        lock (_syncLock)
        {
            _tasks.Add(task);
        }

        return task;
    }

    public MarkAsDoneResult MarkAsDone(Guid id)
    {
        lock (_syncLock)
        {
            var task = _tasks.FirstOrDefault(item => item.Id == id);
            if (task is null)
            {
                return MarkAsDoneResult.NotFound;
            }

            if (task.Status == TaskStatus.Done)
            {
                return MarkAsDoneResult.AlreadyDone;
            }

            task.Status = TaskStatus.Done;
            return MarkAsDoneResult.Updated;
        }
    }
}

public enum MarkAsDoneResult
{
    Updated,
    NotFound,
    AlreadyDone
}
