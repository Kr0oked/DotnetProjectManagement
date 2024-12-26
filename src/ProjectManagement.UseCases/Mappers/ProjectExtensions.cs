namespace DotnetProjectManagement.ProjectManagement.UseCases.Mappers;

using System.Collections.Immutable;
using Domain.Entities;
using DTOs;

internal static class ProjectExtensions
{
    public static ProjectDto ToDto(this Project project) => new()
    {
        Id = project.Id,
        DisplayName = project.DisplayName,
        Archived = project.Archived,
        Members = project.Members.ToImmutableDictionary()
    };
}
