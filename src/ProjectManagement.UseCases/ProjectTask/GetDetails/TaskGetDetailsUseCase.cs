namespace DotnetProjectManagement.ProjectManagement.UseCases.ProjectTask.GetDetails;

using Abstractions;
using Domain.Entities;
using DTOs;
using Exceptions;
using Extensions;

public class TaskGetDetailsUseCase(ITaskRepository taskRepository, IProjectRepository projectRepository)
{
    public async Task<TaskDto> GetTaskDetailsAsync(
        Actor actor,
        Guid taskId,
        CancellationToken cancellationToken = default)
    {
        var task = await this.GetTask(taskId, cancellationToken);
        var project = await this.GetProject(task, cancellationToken);
        actor.VerifyIsProjectMember(project);
        return task.ToDto();
    }

    private async Task<ProjectTask> GetTask(Guid taskId, CancellationToken cancellationToken) =>
        await taskRepository.FindOneAsync(taskId, cancellationToken)
        ?? throw new TaskNotFoundException(taskId);

    private async Task<Project> GetProject(ProjectTask task, CancellationToken cancellationToken) =>
        await projectRepository.FindOneAsync(task.ProjectId, cancellationToken)
        ?? throw new ProjectNotFoundException(task.ProjectId);
}
