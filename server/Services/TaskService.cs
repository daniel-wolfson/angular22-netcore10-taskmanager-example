using TaskManager.Api.Models;
using TaskManager.Api.Requests;
using TaskManager.Api.Services.Results;
using TaskStatus = TaskManager.Api.Models.TaskStatus;

namespace TaskManager.Api.Services;

public class TaskService : ITaskService
{
    private readonly object _syncLock = new();
    private readonly List<TaskItem> _tasks = [];

    /// <summary>
    /// Retrieves all tasks, optionally filtered by title, 
    /// ordered by creation date descending.
    /// </summary>
    public IReadOnlyList<TaskItem> GetTasks(string? title)
    {
        lock (_syncLock)
        {
            var query = _tasks.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(title))
            {
                query = query.Where(task => 
                    task.Title.Contains(title, StringComparison.OrdinalIgnoreCase));
            }

            return query
                .OrderByDescending(task => task.CreatedDate)
                .ToList();
        }
    }

    /// <summary>
    /// Creates a new task with the specified title and optional description.
    /// The task is initialized with an Open status and the current UTC timestamp.
    /// </summary>
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

    /// <summary>
    /// Marks a task as done by its unique identifier.
    /// </summary>
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

    /// <summary>
    /// Gets a task by its unique identifier.
    /// </summary>
    public TaskItem? GetTaskById(Guid id)
    {
        lock (_syncLock)
        {
            return _tasks.FirstOrDefault(item => item.Id == id);
        }
    }
}
