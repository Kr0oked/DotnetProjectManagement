namespace DotnetProjectManagement.ProjectManagement.UseCases.ProjectTask.Reopen;

using Abstractions;
using Domain.Actions;
using Domain.Entities;
using DTOs;
using Exceptions;
using Extensions;
using Microsoft.Extensions.Logging;

public class TaskReopenUseCase(
    ITaskRepository taskRepository,
    IProjectRepository projectRepository,
    ITransactionManager transactionManager,
    ILogger<TaskReopenUseCase> logger)
{
    public async Task<TaskDto> ReopenTaskAsync(Actor actor, Guid taskId, CancellationToken cancellationToken = default)
    {
        await using var transaction = await transactionManager.BeginTransactionAsync(cancellationToken);
        var task = await this.GetTask(taskId, cancellationToken);
        var project = await this.GetProject(task, cancellationToken);

        VerifyActorIsAllowedToReopenTask(actor, task, project);
        VerifyTaskIsClosed(taskId, task);
        project.VerifyProjectIsNotArchived();

        task.Open = true;

        await taskRepository.SaveAsync(task, TaskAction.Reopen, actor.UserId, cancellationToken);

        await transaction.CommitAsync(cancellationToken);
        logger.LogTaskReopened(actor.UserId, taskId);
        return task.ToDto();
    }

    private async Task<ProjectTask> GetTask(Guid taskId, CancellationToken cancellationToken) =>
        await taskRepository.FindOneAsync(taskId, cancellationToken)
        ?? throw new TaskNotFoundException(taskId);

    private async Task<Project> GetProject(ProjectTask task, CancellationToken cancellationToken) =>
        await projectRepository.FindOneAsync(task.ProjectId, cancellationToken)
        ?? throw new ProjectNotFoundException(task.ProjectId);

    private static void VerifyActorIsAllowedToReopenTask(Actor actor, ProjectTask task, Project project)
    {
        if (!task.Assignees.Contains(actor.UserId))
        {
            actor.VerifyIsManager(project);
        }
    }

    private static void VerifyTaskIsClosed(Guid taskId, ProjectTask task)
    {
        if (task.Open)
        {
            throw new TaskOpenException(taskId);
        }
    }
}
