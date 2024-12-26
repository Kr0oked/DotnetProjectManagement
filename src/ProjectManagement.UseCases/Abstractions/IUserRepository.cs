namespace DotnetProjectManagement.ProjectManagement.UseCases.Abstractions;

using Domain.Entities;

public interface IUserRepository
{
    public Task<Page<User>> FindAllAsync(
        Guid userId,
        PageRequest pageRequest,
        CancellationToken cancellationToken = default);

    public Task<bool> ExistsAsync(Guid userId, CancellationToken cancellationToken = default);

    public Task<User?> FindOneAsync(Guid userId, CancellationToken cancellationToken = default);
}
