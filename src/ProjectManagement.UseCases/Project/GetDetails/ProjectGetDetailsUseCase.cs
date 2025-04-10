namespace DotnetProjectManagement.ProjectManagement.UseCases.Project.GetDetails;

using Abstractions;
using Domain.Entities;
using DTOs;
using Exceptions;
using Mappers;

public class ProjectGetDetailsUseCase(IProjectRepository projectRepository)
{
    public async Task<ProjectDto> GetProjectDetailsAsync(
        Actor actor,
        Guid projectId,
        CancellationToken cancellationToken = default)
    {
        var project = await this.GetProject(projectId, cancellationToken);
        VerifyAuthorization(actor, projectId, project);
        return project.ToDto();
    }

    private async Task<Project> GetProject(Guid projectId, CancellationToken cancellationToken)
    {
        var project = await projectRepository.FindOneAsync(projectId, cancellationToken)
                      ?? throw new ProjectNotFoundException(projectId);
        return project;
    }

    private static void VerifyAuthorization(Actor actor, Guid projectId, Project project)
    {
        if (!actor.IsAdministrator && project.GetRoleOfUser(actor.UserId) is null)
        {
            throw new ProjectMemberException(actor, projectId);
        }
    }
}
