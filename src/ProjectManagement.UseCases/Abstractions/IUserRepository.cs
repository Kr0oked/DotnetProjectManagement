namespace DotnetProjectManagement.ProjectManagement.UseCases.Abstractions;

using Domain.Entities;
using DTOs;

public interface IUserRepository
{
    public Task<Page<User>> FindAllAsync(PageRequest pageRequest, CancellationToken cancellationToken = default);

    public Task<bool> ExistsAsync(Guid userId, CancellationToken cancellationToken = default);

    public Task<User?> FindOneAsync(Guid userId, CancellationToken cancellationToken = default);
}
