namespace DotnetProjectManagement.ProjectManagement.UseCases.ProjectTask.Close;

using Abstractions;
using Domain.Actions;
using Domain.Entities;
using DTOs;
using Exceptions;
using Extensions;
using Microsoft.Extensions.Logging;

public class TaskCloseUseCase(
    ITaskRepository taskRepository,
    IProjectRepository projectRepository,
    ITransactionManager transactionManager,
    IMessageBroker messageBroker,
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

        await taskRepository.SaveAsync(task, TaskAction.Close, actor.UserId, cancellationToken);

        await transaction.CommitAsync(cancellationToken);
        logger.LogTaskClosed(actor.UserId, taskId);

        await this.PublishMessageAsync(actor, task, project, cancellationToken);

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

    private async Task PublishMessageAsync(
        Actor actor,
        ProjectTask task,
        Project project,
        CancellationToken cancellationToken)
    {
        var taskActionMessage = new TaskActionMessage
        {
            ActorUserId = actor.UserId,
            Action = TaskAction.Close,
            Task = task.ToDto(),
            Project = project.ToDto()
        };
        await messageBroker.Publish(taskActionMessage, cancellationToken);
    }
}
