namespace DotnetProjectManagement.ProjectManagement.UseCases.Abstractions;

using Domain.Entities;

public interface IProjectRepository
{
    public Task<Page<Project>> FindAllAsync(
        PageRequest pageRequest,
        CancellationToken cancellationToken = default);

    public Task<Page<Project>> FindByMembershipAsync(
        Guid userId,
        PageRequest pageRequest,
        CancellationToken cancellationToken = default);

    public Task<Project?> FindOneAsync(Guid projectId, CancellationToken cancellationToken = default);

    public Task SaveAsync(Project project, CancellationToken cancellationToken = default);
}
