namespace DotnetProjectManagement.ProjectManagement.UseCases.Project.Restore;

using Abstractions;
using Domain.Entities;
using Microsoft.Extensions.Logging;

public class ProjectRestoreUseCase(
    IProjectRepository projectRepository,
    IActivityRepository activityRepository,
    ITransactionManager transactionManager,
    TimeProvider timeProvider,
    ILogger<ProjectRestoreUseCase> logger)
{
    public async Task RestoreProjectAsync(
        Actor actor,
        Guid projectId,
        CancellationToken cancellationToken = default
    )
    {
        await using var transaction = await transactionManager.BeginTransactionAsync(cancellationToken);

        var project = await projectRepository.FindOneAsync(projectId, cancellationToken)
                      ?? throw new ProjectNotFoundException(projectId);

        if (!actor.IsAdministrator && project.GetRoleOfUser(actor.UserId) != ProjectMemberRole.Manager)
        {
            throw new ManagerRequiredException(actor, projectId);
        }

        if (!project.Archived)
        {
            throw new ProjectNotArchivedException(projectId);
        }

        project.Archived = false;

        await projectRepository.SaveAsync(project, cancellationToken);

        await this.CreateActivityAsync(actor, project, cancellationToken);

        await transaction.CommitAsync(cancellationToken);
        logger.LogProjectRestored(actor.UserId, projectId);
    }

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
