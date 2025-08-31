namespace DotnetProjectManagement.ProjectManagement.UseCases.ProjectTask.Create;

using System.Collections.Immutable;

public record TaskCreateCommand
{
    public required Guid ProjectId { get; init; }
    public required string DisplayName { get; init; }
    public required string Description { get; init; }
    public required ImmutableHashSet<Guid> Assignees { get; init; }
}
