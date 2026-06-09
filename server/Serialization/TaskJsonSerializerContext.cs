using TaskManager.Api.Models;
using TaskManager.Api.Requests;
using System.Text.Json;
using System.Text.Json.Serialization;

[JsonSourceGenerationOptions(JsonSerializerDefaults.Web, UseStringEnumConverter = true)]
[JsonSerializable(typeof(TaskItem))]
[JsonSerializable(typeof(List<TaskItem>))]
[JsonSerializable(typeof(CreateTaskRequest))]
internal partial class TaskJsonSerializerContext : JsonSerializerContext
{
}
