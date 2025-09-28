namespace DotnetProjectManagement.ProjectManagement.UseCases.Project.GetHistory;

using Domain.Entities;
using Abstractions;
using Domain.Actions;
using DTOs;
using Exceptions;
using Extensions;

public class ProjectGetHistoryUseCase(IProjectRepository projectRepository)
{
    public async Task<List<HistoryEntry<ProjectAction, ProjectDto>>> GetProjectHistoryAsync(
        Actor actor,
        Guid projectId,
        CancellationToken cancellationToken = default)
    {
        var project = await this.GetProject(projectId, cancellationToken);
        actor.VerifyIsProjectMember(project);
        var history = await projectRepository.GetHistory(projectId, cancellationToken);
        return MapToDto(history);
    }

    private async Task<Project> GetProject(Guid projectId, CancellationToken cancellationToken) =>
        await projectRepository.FindOneAsync(projectId, cancellationToken)
        ?? throw new ProjectNotFoundException(projectId);

    private static List<HistoryEntry<ProjectAction, ProjectDto>> MapToDto(
        List<HistoryEntry<ProjectAction, Project>> history) =>
        [.. history.Select(entry => entry.Map(state => state.ToDto()))];
}
