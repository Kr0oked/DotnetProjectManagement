namespace DotnetProjectManagement.ProjectManagement.Domain.Entities;

public abstract class ProjectActivity : Activity
{
    public required Guid ProjectId { get; init; }
}
