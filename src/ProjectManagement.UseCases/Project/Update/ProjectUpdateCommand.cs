namespace DotnetProjectManagement.ProjectManagement.UseCases.Project.Update;

using System.Collections.Immutable;
using Domain.Entities;

public record ProjectUpdateCommand
{
    public required Guid ProjectId { get; init; }
    public required string DisplayName { get; init; }
    public required ImmutableDictionary<Guid, ProjectMemberRole> Members { get; init; }
}
