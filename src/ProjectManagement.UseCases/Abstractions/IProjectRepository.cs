namespace DotnetProjectManagement.ProjectManagement.UseCases.Abstractions;

using Domain.Actions;
using Domain.Entities;
using DTOs;

public interface IProjectRepository
{
    public Task<Page<Project>> FindAllAsync(
        PageRequest pageRequest,
        CancellationToken cancellationToken = default);

    public Task<Page<Project>> FindByNotArchivedAndMembershipAsync(
        Guid userId,
        PageRequest pageRequest,
        CancellationToken cancellationToken = default);

    public Task<Project?> FindOneAsync(Guid projectId, CancellationToken cancellationToken = default);

    public Task<List<HistoryEntry<ProjectAction, Project>>> GetHistory(
        Guid projectId,
        CancellationToken cancellationToken = default);

    public Task<Project> SaveAsync(
        Project project,
        ProjectAction action,
        Guid actorUserId,
        CancellationToken cancellationToken = default);
}
