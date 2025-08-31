namespace DotnetProjectManagement.ProjectManagement.Domain.Entities;

public abstract class TaskActivity : Activity
{
    public required Guid TaskId { get; init; }
}
