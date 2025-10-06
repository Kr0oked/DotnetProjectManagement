namespace DotnetProjectManagement.ProjectManagement.UseCases.ProjectTask.Update;

using System.ComponentModel.DataAnnotations;
using Abstractions;
using Domain.Actions;
using Domain.Entities;
using DTOs;
using Exceptions;
using Extensions;
using Microsoft.Extensions.Logging;

public class TaskUpdateUseCase(
    ITaskRepository taskRepository,
    IProjectRepository projectRepository,
    IUserRepository userRepository,
    ITransactionManager transactionManager,
    IMessageBroker messageBroker,
    ILogger<TaskUpdateUseCase> logger)
{
    public async Task<TaskDto> UpdateTaskAsync(
        Actor actor,
        TaskUpdateCommand command,
        CancellationToken cancellationToken = default)
    {
        await using var transaction = await transactionManager.BeginTransactionAsync(cancellationToken);

        var task = await this.GetTask(command.TaskId, cancellationToken);
        var project = await this.GetProject(task, cancellationToken);

        await this.VerifyUsersExist(command, cancellationToken);
        actor.VerifyIsManager(project);
        project.VerifyProjectIsNotArchived();

        await this.UpdateEntityAsync(actor, command, task, cancellationToken);

        await transaction.CommitAsync(cancellationToken);
        logger.LogInformation("User {UserId} updated {Task}", actor.UserId, task);

        await this.PublishMessageAsync(actor, task, project, cancellationToken);

        return task.ToDto();
    }

    private async Task<ProjectTask> GetTask(Guid taskId, CancellationToken cancellationToken) =>
        await taskRepository.FindOneAsync(taskId, cancellationToken)
        ?? throw new TaskNotFoundException(taskId);

    private async Task<Project> GetProject(ProjectTask task, CancellationToken cancellationToken) =>
        await projectRepository.FindOneAsync(task.ProjectId, cancellationToken)
        ?? throw new ProjectNotFoundException(task.ProjectId);

    private async Task VerifyUsersExist(TaskUpdateCommand command, CancellationToken cancellationToken)
    {
        foreach (var assignee in command.Assignees)
        {
            if (!await userRepository.ExistsAsync(assignee, cancellationToken))
            {
                throw new UserNotFoundException(assignee);
            }
        }
    }

    private async Task UpdateEntityAsync(
        Actor actor,
        TaskUpdateCommand command,
        ProjectTask task,
        CancellationToken cancellationToken)
    {
        task.DisplayName = command.DisplayName;
        task.Description = command.Description;
        task.Assignees = [.. command.Assignees];

        Validator.ValidateObject(task, new ValidationContext(task), true);

        await taskRepository.SaveAsync(task, TaskAction.Update, actor.UserId, cancellationToken);
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
            Action = TaskAction.Update,
            Task = task.ToDto(),
            Project = project.ToDto()
        };
        await messageBroker.Publish(taskActionMessage, cancellationToken);
    }
}
