namespace DotnetProjectManagement.ProjectManagement.UseCases.Project.Create;

using System.ComponentModel.DataAnnotations;
using Abstractions;
using Domain.Actions;
using Domain.Entities;
using DTOs;
using Exceptions;
using Extensions;
using Microsoft.Extensions.Logging;

public class ProjectCreateUseCase(
    IProjectRepository projectRepository,
    IUserRepository userRepository,
    ITransactionManager transactionManager,
    IMessageBroker messageBroker,
    ILogger<ProjectCreateUseCase> logger)
{
    public async Task<ProjectDto> CreateProjectAsync(
        Actor actor,
        ProjectCreateCommand command,
        CancellationToken cancellationToken = default)
    {
        await using var transaction = await transactionManager.BeginTransactionAsync(cancellationToken);

        actor.VerifyIsAdministrator();
        await this.VerifyUsersExist(command, cancellationToken);

        var project = await this.CreateEntityAsync(actor, command, cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        logger.LogInformation("User {UserId} created {Project}", actor.UserId, project);

        await this.PublishMessageAsync(actor, project, cancellationToken);

        return project.ToDto();
    }

    private async Task VerifyUsersExist(ProjectCreateCommand command, CancellationToken cancellationToken)
    {
        foreach (var member in command.Members)
        {
            if (!await userRepository.ExistsAsync(member.Key, cancellationToken))
            {
                throw new UserNotFoundException(member.Key);
            }
        }
    }

    private async Task<Project> CreateEntityAsync(
        Actor actor,
        ProjectCreateCommand command,
        CancellationToken cancellationToken)
    {
        var project = new Project
        {
            DisplayName = command.DisplayName,
            Archived = false,
            Members = command.Members.ToDictionary()
        };

        Validator.ValidateObject(project, new ValidationContext(project), true);

        return await projectRepository.SaveAsync(project, ProjectAction.Create, actor.UserId, cancellationToken);
    }

    private async Task PublishMessageAsync(Actor actor, Project project, CancellationToken cancellationToken)
    {
        var projectActionMessage = new ProjectActionMessage
        {
            ActorUserId = actor.UserId,
            Action = ProjectAction.Create,
            Project = project.ToDto()
        };
        await messageBroker.Publish(projectActionMessage, cancellationToken);
    }
}
