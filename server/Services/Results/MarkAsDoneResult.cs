namespace TaskManager.Api.Services.Results;

/// <summary>
/// Represents the possible outcomes of a MarkAsDone operation.
/// </summary>
public enum MarkAsDoneResult
{
    /// <summary>
    /// Task was successfully marked as done.
    /// </summary>
    Updated,

    /// <summary>
    /// Task with the specified ID was not found.
    /// </summary>
    NotFound,

    /// <summary>
    /// Task was already in a Done status.
    /// </summary>
    AlreadyDone
}
