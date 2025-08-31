namespace DotnetProjectManagement.ProjectManagement.UseCases.Project.GetDetails;

using Abstractions;
using Domain.Entities;
using DTOs;
using Exceptions;
using Extensions;

public class ProjectGetDetailsUseCase(IProjectRepository projectRepository)
{
    public async Task<ProjectDto> GetProjectDetailsAsync(
        Actor actor,
        Guid projectId,
        CancellationToken cancellationToken = default)
    {
        var project = await this.GetProject(projectId, cancellationToken);
        actor.VerifyIsProjectMember(project);
        return project.ToDto();
    }

    private async Task<Project> GetProject(Guid projectId, CancellationToken cancellationToken) =>
        await projectRepository.FindOneAsync(projectId, cancellationToken)
        ?? throw new ProjectNotFoundException(projectId);
}
