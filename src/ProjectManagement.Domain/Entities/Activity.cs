namespace DotnetProjectManagement.ProjectManagement.Domain.Entities;

public abstract class Activity
{
    public required Guid UserId { get; init; }

    public required DateTimeOffset Timestamp { get; init; }
}
