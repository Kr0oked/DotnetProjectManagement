namespace DotnetProjectManagement.ProjectManagement.UseCases.DTOs;

using System.Collections.Immutable;
using Domain.Entities;

public record ProjectDto
{
    public required Guid Id { get; init; }

    public required string DisplayName { get; init; }

    public required bool Archived { get; init; }

    public required ImmutableDictionary<Guid, ProjectMemberRole> Members { get; init; }
}
