namespace DotnetProjectManagement.ProjectManagement.UseCases.Project.Create;

using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using UseCases;
using Abstractions;
using Domain.Entities;
using DTOs;
using Mappers;
using Microsoft.Extensions.Logging;

public class ProjectCreateUseCase(
    IProjectRepository projectRepository,
    IUserRepository userRepository,
    IActivityRepository activityRepository,
    ITransactionManager transactionManager,
    TimeProvider timeProvider,
    ILogger<ProjectCreateUseCase> logger)
{
    public async Task<ProjectDto> CreateProjectAsync(
        Actor actor,
        ProjectCreateCommand command,
        CancellationToken cancellationToken = default)
    {
        VerifyActorIsAdministrator(actor);
        await using var transaction = await transactionManager.BeginTransactionAsync(cancellationToken);
        var project = await this.CreateProjectAsync(command, cancellationToken);
        await this.CreateActivityAsync(actor, project, cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        logger.LogProjectCreated(actor.UserId, project);
        return project.ToDto();
    }

    private static void VerifyActorIsAdministrator(Actor actor)
    {
        if (!actor.IsAdministrator)
        {
            throw new AdministratorRequiredException(actor);
        }
    }

    private async Task<Project> CreateProjectAsync(ProjectCreateCommand command, CancellationToken cancellationToken)
    {
        foreach (var member in command.Members)
        {
            if (!await userRepository.ExistsAsync(member.Key, cancellationToken))
            {
                throw new UserNotFoundException(member.Key);
            }
        }

        var project = new Project
        {
            Id = Guid.NewGuid(),
            DisplayName = command.DisplayName,
            Archived = false,
            Members = command.Members.ToDictionary()
        };

        Validator.ValidateObject(project, new ValidationContext(project));

        await projectRepository.SaveAsync(project, cancellationToken);
        return project;
    }

    private async Task CreateActivityAsync(Actor actor, Project project, CancellationToken cancellationToken)
    {
        var activity = new ProjectCreatedActivity
        {
            UserId = actor.UserId,
            Timestamp = timeProvider.GetUtcNow(),
            ProjectId = project.Id,
            DisplayName = project.DisplayName,
            Members = project.Members.ToImmutableDictionary()
        };

        await activityRepository.SaveAsync(activity, cancellationToken);
    }
}
