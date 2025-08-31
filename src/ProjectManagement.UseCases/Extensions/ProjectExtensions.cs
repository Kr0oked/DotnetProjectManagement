namespace DotnetProjectManagement.ProjectManagement.UseCases.Extensions;

using System.Collections.Immutable;
using Domain.Entities;
using DTOs;
using Exceptions;
using Project.Restore;

public static class ProjectExtensions
{
    public static void VerifyProjectIsArchived(this Project project)
    {
        if (!project.Archived)
        {
            throw new ProjectNotArchivedException(project.Id);
        }
    }

    public static void VerifyProjectIsNotArchived(this Project project)
    {
        if (project.Archived)
        {
            throw new ProjectArchivedException(project.Id);
        }
    }

    public static ProjectDto ToDto(this Project project) => new()
    {
        Id = project.Id,
        DisplayName = project.DisplayName,
        Archived = project.Archived,
        Members = project.Members.ToImmutableDictionary()
    };
}
