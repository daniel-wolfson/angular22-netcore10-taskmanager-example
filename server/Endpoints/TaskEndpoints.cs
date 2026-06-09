using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using TaskManager.Api.Requests;
using TaskManager.Api.Services;
using TaskManager.Api.Services.Results;

namespace TaskManager.Api.Endpoints;

public static class TaskEndpoints
{
    public static RouteGroupBuilder MapTaskEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/tasks")
            .WithTags("Tasks");

        group.MapGet("", GetTasks)
            .WithName("GetTasks")
            .WithSummary("Get all tasks")
            .WithDescription("Returns all tasks, optionally filtered by title.");

        group.MapPost("", CreateTask)
            .WithName("CreateTask")
            .WithSummary("Create a new task")
            .WithDescription("Creates a new task and returns the created resource.");

        group.MapPatch("{id:guid}/status", MarkTaskAsDone)
            .WithName("MarkTaskAsDone")
            .WithSummary("Mark a task as done")
            .WithDescription("Updates the status of the specified task to Done.");

        return group;
    }

    private static IResult GetTasks(string? title, ITaskService taskService, ILogger<TaskService> logger)
    {
        logger.LogInformation("GetTasks endpoint called with title filter: {Title}", title ?? "null");

        try
        {
            var tasks = taskService.GetTasks(title);

            logger.LogInformation("GetTasks returned {Count} tasks", tasks.Count);

            return Results.Ok(tasks);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while getting tasks with filter: {Title}", title ?? "null");
            return Results.Problem("An error occurred while retrieving tasks.");
        }
    }

    private static async Task<IResult> CreateTask(
        CreateTaskRequest request,
        IValidator<CreateTaskRequest> validator,
        ITaskService taskService,
        ILogger<TaskService> logger)
    {
        logger.LogInformation("CreateTask endpoint called with title: {Title}, description: {Description}", 
            request.Title, request.Description);

        try
        {
            var validation = await validator.ValidateAsync(request);
            if (!validation.IsValid)
            {
                logger.LogWarning("CreateTask validation failed: {Errors}", 
                    string.Join(", ", validation.Errors.Select(e => e.ErrorMessage)));
                return Results.ValidationProblem(validation.ToDictionary());
            }

            var task = taskService.CreateTask(request);

            logger.LogInformation("Task created successfully with ID: {TaskId}", task.Id);

            return Results.Created($"/api/tasks/{task.Id}", task);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while creating task with title: {Title}", request.Title);
            return Results.Problem("An error occurred while creating the task.");
        }
    }

    private static IResult MarkTaskAsDone(Guid id, ITaskService taskService, ILogger<TaskService> logger)
    {
        logger.LogInformation("MarkTaskAsDone endpoint called for task ID: {TaskId}", id);

        try
        {
            var result = taskService.MarkAsDone(id);

            if (result == MarkAsDoneResult.NotFound)
            {
                logger.LogWarning("Task not found with ID: {TaskId}", id);
                return Results.NotFound();
            }

            if (result == MarkAsDoneResult.AlreadyDone)
            {
                logger.LogWarning("Task already marked as done: {TaskId}", id);
                return Results.BadRequest(new { message = "Task is already done." });
            }

            logger.LogInformation("Task marked as done successfully: {TaskId}", id);
            return Results.Ok(taskService.GetTaskById(id));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while marking task as done: {TaskId}", id);
            return Results.Problem("An error occurred while updating the task status.");
        }
    }
}
