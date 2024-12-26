namespace DotnetProjectManagement.ProjectManagement.UseCases.Project.List;

using Abstractions;
using Domain.Entities;
using DTOs;
using Mappers;

public class ProjectListUseCase(IProjectRepository projectRepository)
{
    public async Task<Page<ProjectDto>> ListProjectsAsync(
        Actor actor,
        PageRequest pageRequest,
        CancellationToken cancellationToken = default)
    {
        var page = await this.GetProjectPage(actor, pageRequest, cancellationToken);
        return page.Map(project => project.ToDto());
    }

    private async Task<Page<Project>> GetProjectPage(
        Actor actor,
        PageRequest pageRequest,
        CancellationToken cancellationToken) =>
        actor.IsAdministrator
            ? await projectRepository.FindAllAsync(pageRequest, cancellationToken)
            : await projectRepository.FindByMembershipAsync(actor.UserId, pageRequest, cancellationToken);
}
