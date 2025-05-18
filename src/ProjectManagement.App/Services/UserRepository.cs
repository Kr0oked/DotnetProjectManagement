namespace DotnetProjectManagement.ProjectManagement.App.Services;

using System.Collections.Immutable;
using System.Net;
using Data.Contexts;
using Domain.Entities;
using FS.Keycloak.RestApiClient.Client;
using FS.Keycloak.RestApiClient.Model;
using Keycloak;
using Microsoft.Extensions.Options;
using UseCases.Abstractions;
using UseCases.DTOs;

public class UserRepository(
    ProjectManagementDbContext dbContext,
    IKeycloakClientFactory keycloakClientFactory,
    IOptions<UserOptions> options)
    : IUserRepository
{
    public async Task<Page<User>> FindAllAsync(PageRequest pageRequest, CancellationToken cancellationToken = default)
    {
        var usersApi = keycloakClientFactory.GetUsersApi();

        var totalElements = await usersApi
            .GetUsersCountAsync(options.Value.Realm, cancellationToken: cancellationToken);

        var keycloakUsers = await usersApi.GetUsersAsync(
            options.Value.Realm,
            true,
            first: pageRequest.Offset,
            max: pageRequest.Size,
            cancellationToken: cancellationToken);

        var users = keycloakUsers
            .Select(representation => new User
            {
                Id = new Guid(representation.Id),
                FirstName = representation.FirstName,
                LastName = representation.LastName
            })
            .ToImmutableList();

        return new Page<User>(users, pageRequest, totalElements);
    }

    public async Task<bool> ExistsAsync(Guid userId, CancellationToken cancellationToken = default) =>
        await this.DbUserExistsAsync(userId, cancellationToken) ||
        await this.KeycloakUserExistsAsync(userId, cancellationToken);

    public async Task<User?> FindOneAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var keycloakUser = await this.FindKeycloakUserAsync(userId, cancellationToken);

        if (keycloakUser is not null)
        {
            return new User
            {
                Id = new Guid(keycloakUser.Id),
                FirstName = keycloakUser.FirstName,
                LastName = keycloakUser.LastName
            };
        }

        var dbUser = await dbContext.Users
            .FindAsync([userId], cancellationToken);

        return dbUser is not null ? new User { Id = dbUser.Id } : null;
    }

    private async Task<bool> KeycloakUserExistsAsync(Guid userId, CancellationToken cancellationToken)
    {
        var keycloakUser = await this.FindKeycloakUserAsync(userId, cancellationToken);
        return keycloakUser is not null;
    }

    private async Task<bool> DbUserExistsAsync(Guid userId, CancellationToken cancellationToken)
    {
        var dbUser = await dbContext.Users
            .FindAsync([userId], cancellationToken);
        return dbUser is not null;
    }

    private async Task<UserRepresentation?> FindKeycloakUserAsync(Guid userId, CancellationToken cancellationToken)
    {
        var usersApi = keycloakClientFactory.GetUsersApi();

        try
        {
            var keycloakUser = await usersApi.GetUsersByUserIdAsync(
                options.Value.Realm,
                userId.ToString(),
                cancellationToken: cancellationToken);
            return keycloakUser;
        }
        catch (ApiException exception) when (exception.ErrorCode == (int)HttpStatusCode.NotFound)
        {
            return null;
        }
    }
}
