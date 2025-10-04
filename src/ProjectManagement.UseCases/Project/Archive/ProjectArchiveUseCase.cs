namespace DotnetProjectManagement.ProjectManagement.UseCases.Project.Archive;

using Abstractions;
using Domain.Actions;
using Domain.Entities;
using DTOs;
using Exceptions;
using Extensions;
using Microsoft.Extensions.Logging;

public class ProjectArchiveUseCase(
    IProjectRepository projectRepository,
    ITransactionManager transactionManager,
    IMessageBroker messageBroker,
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

        await projectRepository.SaveAsync(project, ProjectAction.Archive, actor.UserId, cancellationToken);

        await transaction.CommitAsync(cancellationToken);
        logger.LogProjectArchived(actor.UserId, projectId);

        await this.PublishMessageAsync(actor, project, cancellationToken);

        return project.ToDto();
    }

    private async Task<Project> GetProject(Guid projectId, CancellationToken cancellationToken) =>
        await projectRepository.FindOneAsync(projectId, cancellationToken)
        ?? throw new ProjectNotFoundException(projectId);

    private async Task PublishMessageAsync(Actor actor, Project project, CancellationToken cancellationToken)
    {
        var projectActionMessage = new ProjectActionMessage
        {
            ActorUserId = actor.UserId,
            Action = ProjectAction.Archive,
            Project = project.ToDto()
        };
        await messageBroker.Publish(projectActionMessage, cancellationToken);
    }
}
