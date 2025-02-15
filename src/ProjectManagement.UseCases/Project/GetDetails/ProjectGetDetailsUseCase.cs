namespace DotnetProjectManagement.ProjectManagement.UseCases.Project.GetDetails;

using Abstractions;
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
        var project = await projectRepository.FindOneAsync(projectId, cancellationToken)
                      ?? throw new ProjectNotFoundException(projectId);

        if (!actor.IsAdministrator && project.GetRoleOfUser(actor.UserId) is null)
        {
            throw new ProjectMemberException(actor, projectId);
        }

        return project.ToDto();
    }
}
