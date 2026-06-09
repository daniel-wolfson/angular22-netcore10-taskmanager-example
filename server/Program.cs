using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Scalar.AspNetCore;
using Serilog;
using TaskManager.Api.Endpoints;
using TaskManager.Api.Requests;
using TaskManager.Api.Services;
using TaskManager.Api.Validators;

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .WriteTo.File("logs/taskmanager-.log",
        rollingInterval: RollingInterval.Day,
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
    .CreateLogger();

try
{
    Log.Information("Starting Task Manager API");

    var builder = WebApplication.CreateBuilder(args);

    // Use Serilog for logging
    builder.Host.UseSerilog();

    builder.Services.AddCors(options =>
    {
        options.AddPolicy("Client", policy =>
        {
            var clientUrl = builder.Configuration["ClientUrl"]
                ?? throw new ArgumentNullException("client url not defined");

            policy
                .WithOrigins(clientUrl)
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
    });

    builder.Services.AddOpenApi();
    builder.Services.AddOpenApi(options =>
    {
        options.AddDocumentTransformer((document, context, _) =>
        {
            document.Info.Title = "Task Manager API";
            document.Info.Version = "v1";
            document.Info.Description = "A simple Task Manager Web API built with ASP.NET Core 10 Minimal APIs.";
            return Task.CompletedTask;
        });
    });

    builder.Services.AddSingleton<ITaskService, TaskService>();
    builder.Services.AddScoped<IValidator<CreateTaskRequest>, CreateTaskRequestValidator>();

    builder.Services.ConfigureHttpJsonOptions(options =>
    {
        options.SerializerOptions.TypeInfoResolverChain.Insert(0, TaskJsonSerializerContext.Default);
    });

    var app = builder.Build();

    // Environment-specific middleware
    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();
        app.MapScalarApiReference(options =>
        {
            options.Title = "Task Manager API";
            options.Theme = ScalarTheme.Purple;
        });
    }

    // Add Cors policy
    app.UseCors("Client");

    // Map endpoints
    app.MapTaskEndpoints();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
