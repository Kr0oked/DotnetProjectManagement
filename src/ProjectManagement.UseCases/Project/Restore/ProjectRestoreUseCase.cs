namespace DotnetProjectManagement.ProjectManagement.UseCases.Project.Restore;

using Abstractions;
using Domain.Entities;
using DTOs;
using Exceptions;
using Extensions;
using Microsoft.Extensions.Logging;

public class ProjectRestoreUseCase(
    IProjectRepository projectRepository,
    IActivityRepository activityRepository,
    ITransactionManager transactionManager,
    TimeProvider timeProvider,
    ILogger<ProjectRestoreUseCase> logger)
{
    public async Task<ProjectDto> RestoreProjectAsync(
        Actor actor,
        Guid projectId,
        CancellationToken cancellationToken = default
    )
    {
        await using var transaction = await transactionManager.BeginTransactionAsync(cancellationToken);
        var project = await this.GetProject(projectId, cancellationToken);

        actor.VerifyIsManager(project);
        project.VerifyProjectIsArchived();

        project.Archived = false;

        await projectRepository.SaveAsync(project, cancellationToken);

        await this.CreateActivityAsync(actor, project, cancellationToken);

        await transaction.CommitAsync(cancellationToken);
        logger.LogProjectRestored(actor.UserId, projectId);
        return project.ToDto();
    }

    private async Task<Project> GetProject(Guid projectId, CancellationToken cancellationToken) =>
        await projectRepository.FindOneAsync(projectId, cancellationToken)
        ?? throw new ProjectNotFoundException(projectId);

    private async Task CreateActivityAsync(Actor actor, Project project, CancellationToken cancellationToken)
    {
        var activity = new ProjectRestoredActivity
        {
            UserId = actor.UserId,
            Timestamp = timeProvider.GetUtcNow(),
            ProjectId = project.Id
        };

        await activityRepository.SaveAsync(activity, cancellationToken);
    }
}
