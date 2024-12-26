namespace DotnetProjectManagement.ProjectManagement.UseCases.Project.Update;

using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using Abstractions;
using DTOs;
using Domain.Entities;
using Mappers;
using Microsoft.Extensions.Logging;

public class ProjectUpdateUseCase(
    IProjectRepository projectRepository,
    IUserRepository userRepository,
    IActivityRepository activityRepository,
    ITransactionManager transactionManager,
    TimeProvider timeProvider,
    ILogger<ProjectUpdateUseCase> logger)
{
    public async Task<ProjectDto> UpdateProjectAsync(
        Actor actor,
        ProjectUpdateCommand command,
        CancellationToken cancellationToken = default)
    {
        await using var transaction = await transactionManager.BeginTransactionAsync(cancellationToken);

        var project = await projectRepository.FindOneAsync(command.ProjectId, cancellationToken)
                      ?? throw new ProjectNotFoundException(command.ProjectId);

        if (!actor.IsAdministrator && project.GetRoleOfUser(actor.UserId) != ProjectMemberRole.Manager)
        {
            throw new ManagerRequiredException(actor, command.ProjectId);
        }

        if (project.Archived)
        {
            throw new ProjectArchivedException(command.ProjectId);
        }

        foreach (var member in command.Members)
        {
            if (!await userRepository.ExistsAsync(member.Key, cancellationToken))
            {
                throw new UserNotFoundException(member.Key);
            }
        }

        await this.CreateActivityAsync(actor, command, project, cancellationToken);
        await this.UpdateProjectAsync(command, project, cancellationToken);

        await transaction.CommitAsync(cancellationToken);
        logger.LogProjectUpdated(actor.UserId, project);
        return project.ToDto();
    }

    private async Task CreateActivityAsync(
        Actor actor,
        ProjectUpdateCommand update,
        Project project,
        CancellationToken cancellationToken)
    {
        var activity = new ProjectUpdatedActivity
        {
            UserId = actor.UserId,
            Timestamp = timeProvider.GetUtcNow(),
            ProjectId = project.Id,
            OldDisplayName = project.DisplayName,
            NewDisplayName = update.DisplayName,
            OldMembers = project.Members.ToImmutableDictionary(),
            NewMembers = update.Members
        };

        await activityRepository.SaveAsync(activity, cancellationToken);
    }

    private async Task UpdateProjectAsync(
        ProjectUpdateCommand command,
        Project project,
        CancellationToken cancellationToken)
    {
        project.DisplayName = command.DisplayName;
        project.Members = command.Members.ToDictionary();

        Validator.ValidateObject(project, new ValidationContext(project));

        await projectRepository.SaveAsync(project, cancellationToken);
    }
}
