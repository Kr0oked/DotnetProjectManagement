namespace DotnetProjectManagement.ProjectManagement.UseCases.DTOs;

using System.Collections.Immutable;

public record TaskDto
{
    public required Guid Id { get; init; }
    public required string DisplayName { get; init; }
    public required string Description { get; init; }
    public required bool Open { get; init; }
    public required ImmutableHashSet<Guid> Assignees { get; init; }
}
