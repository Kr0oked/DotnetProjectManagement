namespace DotnetProjectManagement.ProjectManagement.UseCases.ProjectTask.GetHistory;

using Domain.Actions;
using Domain.Entities;
using Abstractions;
using DTOs;
using Exceptions;
using Extensions;

public class TaskGetHistoryUseCase(ITaskRepository taskRepository, IProjectRepository projectRepository)
{
    public async Task<List<HistoryEntry<TaskAction, TaskDto>>> GetTaskHistoryAsync(
        Actor actor,
        Guid taskId,
        CancellationToken cancellationToken = default)
    {
        var task = await this.GetTask(taskId, cancellationToken);
        var project = await this.GetProject(task, cancellationToken);
        actor.VerifyIsProjectMember(project);
        var history = await taskRepository.GetHistory(taskId, cancellationToken);
        return MapToDto(history);
    }

    private async Task<ProjectTask> GetTask(Guid taskId, CancellationToken cancellationToken) =>
        await taskRepository.FindOneAsync(taskId, cancellationToken)
        ?? throw new TaskNotFoundException(taskId);

    private async Task<Project> GetProject(ProjectTask task, CancellationToken cancellationToken) =>
        await projectRepository.FindOneAsync(task.ProjectId, cancellationToken)
        ?? throw new ProjectNotFoundException(task.ProjectId);

    private static List<HistoryEntry<TaskAction, TaskDto>> MapToDto(
        List<HistoryEntry<TaskAction, ProjectTask>> history) =>
        [.. history.Select(entry => entry.Map(state => state.ToDto()))];
}
