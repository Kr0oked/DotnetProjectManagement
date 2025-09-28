namespace DotnetProjectManagement.ProjectManagement.UseCases.Abstractions;

using Domain.Actions;
using Domain.Entities;
using DTOs;

public interface ITaskRepository
{
    public Task<ProjectTask?> FindOneAsync(Guid taskId, CancellationToken cancellationToken);

    public Task<Page<ProjectTask>> FindAllByProjectIdAsync(
        Guid projectId,
        PageRequest pageRequest,
        CancellationToken cancellationToken = default);

    public Task<List<HistoryEntry<TaskAction, ProjectTask>>> GetHistory(
        Guid taskId,
        CancellationToken cancellationToken = default);

    public Task<ProjectTask> SaveAsync(
        ProjectTask task,
        TaskAction action,
        Guid actorUserId,
        CancellationToken cancellationToken);
}
