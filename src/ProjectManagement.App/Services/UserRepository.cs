namespace DotnetProjectManagement.ProjectManagement.App.Services;

using System.Collections.Immutable;
using Data.Contexts;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using UseCases.Abstractions;
using UseCases.DTOs;
using UserDb = Data.Models.User;

public class UserRepository(ProjectManagementDbContext dbContext) : IUserRepository
{
    public async Task<Page<User>> FindAllAsync(PageRequest pageRequest, CancellationToken cancellationToken = default)
    {
        var queryable = dbContext.Users
            .OrderBy(user => user.CreatedAt);

        var totalElements = await queryable.LongCountAsync(cancellationToken);

        var users = queryable
            .Skip(pageRequest.Offset)
            .Take(pageRequest.Size)
            .Select(user => new User
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName
            })
            .ToImmutableList();

        return new Page<User>(users, pageRequest, totalElements);
    }

    public async Task<bool> ExistsAsync(Guid userId, CancellationToken cancellationToken = default) =>
        await this.FindOneAsync(userId, cancellationToken) is not null;

    public async Task<User?> FindOneAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var userDb = await dbContext.Users
            .FindAsync([userId], cancellationToken);

        return userDb is not null ? MapToEntity(userDb) : null;
    }

    public async Task<User> SaveAsync(User user, CancellationToken cancellationToken)
    {
        var userDb = new UserDb
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName
        };

        dbContext.Users.Add(userDb);
        await dbContext.SaveChangesAsync(cancellationToken);

        return MapToEntity(userDb);
    }

    private static User MapToEntity(UserDb userDb) => new()
    {
        Id = userDb.Id,
        FirstName = userDb.FirstName,
        LastName = userDb.LastName
    };
}
