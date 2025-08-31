namespace DotnetProjectManagement.ProjectManagement.UseCases.ProjectTask.Update;

using System.Collections.Immutable;

public record TaskUpdateCommand
{
    public required Guid TaskId { get; init; }
    public required string DisplayName { get; init; }
    public required string Description { get; init; }
    public required ImmutableHashSet<Guid> Assignees { get; init; }
}
