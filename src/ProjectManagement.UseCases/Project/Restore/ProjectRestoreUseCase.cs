namespace DotnetProjectManagement.ProjectManagement.UseCases.Project.Restore;

using Abstractions;
using Domain.Actions;
using Domain.Entities;
using DTOs;
using Exceptions;
using Extensions;
using Microsoft.Extensions.Logging;

public class ProjectRestoreUseCase(
    IProjectRepository projectRepository,
    ITransactionManager transactionManager,
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

        await projectRepository.SaveAsync(project, ProjectAction.Restore, actor.UserId, cancellationToken);

        await transaction.CommitAsync(cancellationToken);
        logger.LogProjectRestored(actor.UserId, projectId);
        return project.ToDto();
    }

    private async Task<Project> GetProject(Guid projectId, CancellationToken cancellationToken) =>
        await projectRepository.FindOneAsync(projectId, cancellationToken)
        ?? throw new ProjectNotFoundException(projectId);
}
