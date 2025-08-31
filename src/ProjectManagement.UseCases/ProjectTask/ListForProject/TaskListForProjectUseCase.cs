namespace DotnetProjectManagement.ProjectManagement.UseCases.ProjectTask.ListForProject;

using Abstractions;
using Domain.Entities;
using DTOs;
using Exceptions;
using Extensions;

public class TaskListForProjectUseCase(ITaskRepository taskRepository, IProjectRepository projectRepository)
{
    public async Task<Page<TaskDto>> ListTasksForProjectAsync(Actor actor,
        Guid projectId,
        PageRequest pageRequest,
        CancellationToken cancellationToken = default)
    {
        var project = await this.GetProject(projectId, cancellationToken);
        actor.VerifyIsProjectMember(project);

        var page = await taskRepository.FindAllByProjectIdAsync(projectId, pageRequest, cancellationToken);

        return page.Map(task => task.ToDto());
    }

    private async Task<Project> GetProject(Guid projectId, CancellationToken cancellationToken) =>
        await projectRepository.FindOneAsync(projectId, cancellationToken)
        ?? throw new ProjectNotFoundException(projectId);
}
