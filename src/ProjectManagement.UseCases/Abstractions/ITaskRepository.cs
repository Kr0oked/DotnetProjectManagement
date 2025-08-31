namespace DotnetProjectManagement.ProjectManagement.UseCases.Abstractions;

using Domain.Entities;
using DTOs;

public interface ITaskRepository
{
    public Task<ProjectTask?> FindOneAsync(Guid taskId, CancellationToken cancellationToken);

    public Task<Page<ProjectTask>> FindAllByProjectIdAsync(
        Guid projectId,
        PageRequest pageRequest,
        CancellationToken cancellationToken = default);

    public Task SaveAsync(ProjectTask task, CancellationToken cancellationToken);
}
