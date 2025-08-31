namespace DotnetProjectManagement.ProjectManagement.UseCases.Project.Archive;

using Abstractions;
using Domain.Entities;
using DTOs;
using Exceptions;
using Extensions;
using Microsoft.Extensions.Logging;

public class ProjectArchiveUseCase(
    IProjectRepository projectRepository,
    IActivityRepository activityRepository,
    ITransactionManager transactionManager,
    TimeProvider timeProvider,
    ILogger<ProjectArchiveUseCase> logger)
{
    public async Task<ProjectDto> ArchiveProjectAsync(
        Actor actor,
        Guid projectId,
        CancellationToken cancellationToken = default
    )
    {
        await using var transaction = await transactionManager.BeginTransactionAsync(cancellationToken);
        var project = await this.GetProject(projectId, cancellationToken);

        actor.VerifyIsManager(project);
        project.VerifyProjectIsNotArchived();

        project.Archived = true;

        await projectRepository.SaveAsync(project, cancellationToken);

        await this.CreateActivityAsync(actor, projectId, cancellationToken);

        await transaction.CommitAsync(cancellationToken);
        logger.LogProjectArchived(actor.UserId, projectId);
        return project.ToDto();
    }

    private async Task<Project> GetProject(Guid projectId, CancellationToken cancellationToken) =>
        await projectRepository.FindOneAsync(projectId, cancellationToken)
        ?? throw new ProjectNotFoundException(projectId);

    private async Task CreateActivityAsync(Actor actor, Guid projectId, CancellationToken cancellationToken)
    {
        var activity = new ProjectArchivedActivity
        {
            UserId = actor.UserId,
            Timestamp = timeProvider.GetUtcNow(),
            ProjectId = projectId
        };

        await activityRepository.SaveAsync(activity, cancellationToken);
    }
}
