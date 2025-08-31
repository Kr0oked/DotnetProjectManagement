namespace DotnetProjectManagement.ProjectManagement.UseCases.ProjectTask.Close;

using Abstractions;
using Domain.Entities;
using DTOs;
using Exceptions;
using Extensions;
using Microsoft.Extensions.Logging;

public class TaskCloseUseCase(
    ITaskRepository taskRepository,
    IProjectRepository projectRepository,
    IActivityRepository activityRepository,
    ITransactionManager transactionManager,
    TimeProvider timeProvider,
    ILogger<TaskCloseUseCase> logger)
{
    public async Task<TaskDto> CloseTaskAsync(Actor actor, Guid taskId, CancellationToken cancellationToken = default)
    {
        await using var transaction = await transactionManager.BeginTransactionAsync(cancellationToken);
        var task = await this.GetTask(taskId, cancellationToken);
        var project = await this.GetProject(task, cancellationToken);

        VerifyActorIsAllowedToCloseTask(actor, task, project);
        VerifyTaskIsOpen(taskId, task);
        project.VerifyProjectIsNotArchived();

        task.Open = false;

        await taskRepository.SaveAsync(task, cancellationToken);

        await this.CreateActivityAsync(actor, task.Id, cancellationToken);

        await transaction.CommitAsync(cancellationToken);
        logger.LogTaskClosed(actor.UserId, taskId);
        return task.ToDto();
    }

    private async Task<ProjectTask> GetTask(Guid taskId, CancellationToken cancellationToken) =>
        await taskRepository.FindOneAsync(taskId, cancellationToken)
        ?? throw new TaskNotFoundException(taskId);

    private async Task<Project> GetProject(ProjectTask task, CancellationToken cancellationToken) =>
        await projectRepository.FindOneAsync(task.ProjectId, cancellationToken)
        ?? throw new ProjectNotFoundException(task.ProjectId);

    private static void VerifyActorIsAllowedToCloseTask(Actor actor, ProjectTask task, Project project)
    {
        if (!task.Assignees.Contains(actor.UserId))
        {
            actor.VerifyIsManager(project);
        }
    }

    private static void VerifyTaskIsOpen(Guid taskId, ProjectTask task)
    {
        if (!task.Open)
        {
            throw new TaskClosedException(taskId);
        }
    }

    private async Task CreateActivityAsync(Actor actor, Guid taskId, CancellationToken cancellationToken)
    {
        var activity = new TaskClosedActivity
        {
            UserId = actor.UserId,
            Timestamp = timeProvider.GetUtcNow(),
            TaskId = taskId
        };

        await activityRepository.SaveAsync(activity, cancellationToken);
    }
}
