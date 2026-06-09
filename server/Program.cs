using FluentValidation;
using TaskManager.Api.Models;
using TaskManager.Api.Requests;
using TaskManager.Api.Services;
using TaskManager.Api.Validators;
using System.Text.Json;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("Client", policy =>
    {
        policy
            .WithOrigins("http://localhost:4200")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddSingleton<TaskService>();
builder.Services.AddScoped<IValidator<CreateTaskRequest>, CreateTaskRequestValidator>();

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.TypeInfoResolverChain.Insert(0, TaskJsonSerializerContext.Default);
});

var app = builder.Build();

app.UseCors("Client");

app.MapGet("/api/tasks", (string? title, TaskService taskService) =>
{
    return Results.Ok(taskService.GetTasks(title));
});

app.MapPost("/api/tasks", async (CreateTaskRequest request, IValidator<CreateTaskRequest> validator, TaskService taskService) =>
{
    var validation = await validator.ValidateAsync(request);
    if (!validation.IsValid)
    {
        return Results.ValidationProblem(validation.ToDictionary());
    }

    var task = taskService.CreateTask(request);
    return Results.Created($"/api/tasks/{task.Id}", task);
});

app.MapPatch("/api/tasks/{id:guid}/status", (Guid id, TaskService taskService) =>
{
    var result = taskService.MarkAsDone(id);
    return result switch
    {
        MarkAsDoneResult.NotFound => Results.NotFound(),
        MarkAsDoneResult.AlreadyDone => Results.BadRequest(new { message = "Task is already done." }),
        _ => Results.Ok(taskService.GetTasks(null).First(task => task.Id == id))
    };
});

app.Run();

[JsonSourceGenerationOptions(JsonSerializerDefaults.Web, UseStringEnumConverter = true)]
[JsonSerializable(typeof(TaskItem))]
[JsonSerializable(typeof(List<TaskItem>))]
[JsonSerializable(typeof(CreateTaskRequest))]
internal partial class TaskJsonSerializerContext : JsonSerializerContext
{
}
