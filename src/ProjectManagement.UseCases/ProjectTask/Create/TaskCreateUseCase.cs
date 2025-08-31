namespace DotnetProjectManagement.ProjectManagement.UseCases.ProjectTask.Create;

using System.ComponentModel.DataAnnotations;
using Abstractions;
using Domain.Entities;
using DTOs;
using Exceptions;
using Extensions;
using Microsoft.Extensions.Logging;

public class TaskCreateUseCase(
    ITaskRepository taskRepository,
    IProjectRepository projectRepository,
    IUserRepository userRepository,
    IActivityRepository activityRepository,
    ITransactionManager transactionManager,
    TimeProvider timeProvider,
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

        var task = await this.CreateTaskAsync(command, cancellationToken);
        await this.CreateActivityAsync(actor, task, cancellationToken);

        await transaction.CommitAsync(cancellationToken);

        logger.LogTaskCreated(actor.UserId, task);
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

    private async Task<ProjectTask> CreateTaskAsync(TaskCreateCommand command, CancellationToken cancellationToken)
    {
        var task = new ProjectTask
        {
            DisplayName = command.DisplayName,
            Description = command.Description,
            Open = true,
            Assignees = [.. command.Assignees],
            ProjectId = command.ProjectId
        };

        Validator.ValidateObject(task, new ValidationContext(task), true);

        await taskRepository.SaveAsync(task, cancellationToken);
        return task;
    }

    private async Task CreateActivityAsync(Actor actor, ProjectTask task, CancellationToken cancellationToken)
    {
        var activity = new TaskCreatedActivity
        {
            UserId = actor.UserId,
            Timestamp = timeProvider.GetUtcNow(),
            TaskId = task.Id,
            DisplayName = task.DisplayName,
            Description = task.Description,
            Assignees = [.. task.Assignees]
        };

        await activityRepository.SaveAsync(activity, cancellationToken);
    }
}
