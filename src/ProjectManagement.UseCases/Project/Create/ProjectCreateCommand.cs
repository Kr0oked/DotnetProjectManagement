namespace DotnetProjectManagement.ProjectManagement.UseCases.Project.Create;

using System.Collections.Immutable;
using Domain.Entities;

public record ProjectCreateCommand
{
    public required string DisplayName { get; init; }
    public required ImmutableDictionary<Guid, ProjectMemberRole> Members { get; init; }
}
