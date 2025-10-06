namespace DotnetProjectManagement.ProjectManagement.UseCases.ProjectTask.Create;

using System.ComponentModel.DataAnnotations;
using Abstractions;
using Domain.Actions;
using Domain.Entities;
using DTOs;
using Exceptions;
using Extensions;
using Microsoft.Extensions.Logging;

public class TaskCreateUseCase(
    ITaskRepository taskRepository,
    IProjectRepository projectRepository,
    IUserRepository userRepository,
    ITransactionManager transactionManager,
    IMessageBroker messageBroker,
    ILogger<TaskCreateUseCase> logger)
{
    public async Task<TaskDto> CreateTaskAsync(
        Actor actor,
        TaskCreateCommand command,
        CancellationToken cancellationToken = default)
    {
        await using var transaction = await transactionManager.BeginTransactionAsync(cancellationToken);

        var project = await this.GetProject(command, cancellationToken);

        actor.VerifyIsManager(project);
        project.VerifyProjectIsNotArchived();
        await this.VerifyUsersExist(command, cancellationToken);

        var task = await this.CreateEntityAsync(actor, command, cancellationToken);

        await transaction.CommitAsync(cancellationToken);
        logger.LogInformation("User {UserId} created {Task}", actor.UserId, task);

        await this.PublishMessageAsync(actor, task, project, cancellationToken);

        return task.ToDto();
    }

    private async Task<Project> GetProject(TaskCreateCommand command, CancellationToken cancellationToken) =>
        await projectRepository.FindOneAsync(command.ProjectId, cancellationToken)
        ?? throw new ProjectNotFoundException(command.ProjectId);

    private async Task VerifyUsersExist(TaskCreateCommand command, CancellationToken cancellationToken)
    {
        foreach (var assignee in command.Assignees)
        {
            if (!await userRepository.ExistsAsync(assignee, cancellationToken))
            {
                throw new UserNotFoundException(assignee);
            }
        }
    }

    private async Task<ProjectTask> CreateEntityAsync(
        Actor actor,
        TaskCreateCommand command,
        CancellationToken cancellationToken)
    {
        var task = new ProjectTask
        {
            DisplayName = command.DisplayName,
            Description = command.Description,
            Open = true,
            Assignees = [.. command.Assignees],
            ProjectId = command.ProjectId,
        };

        Validator.ValidateObject(task, new ValidationContext(task), true);

        return await taskRepository.SaveAsync(task, TaskAction.Create, actor.UserId, cancellationToken);
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
            Action = TaskAction.Create,
            Task = task.ToDto(),
            Project = project.ToDto()
        };
        await messageBroker.Publish(taskActionMessage, cancellationToken);
    }
}
