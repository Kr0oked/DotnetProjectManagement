namespace DotnetProjectManagement.ProjectManagement.App.Services;

using System.Collections.Immutable;
using System.Net;
using Keycloak;
using Data.Contexts;
using Domain.Entities;
using UseCases.Abstractions;
using FS.Keycloak.RestApiClient.Client;
using FS.Keycloak.RestApiClient.Model;
using Microsoft.Extensions.Options;
using UseCases.DTOs;
using User;

public class UserRepository(
    ProjectManagementDbContext dbContext,
    KeycloakClientFactory keycloakClientFactory,
    IOptions<UserOptions> options)
    : IUserRepository
{
    public async Task<Page<User>> FindAllAsync(
        Guid userId,
        PageRequest pageRequest,
        CancellationToken cancellationToken = default)
    {
        using var usersApi = keycloakClientFactory.GetUsersApi();

        var totalElements = await usersApi
            .GetUsersCountAsync(realm: options.Value.Realm, cancellationToken: cancellationToken);

        var keycloakUsers = await usersApi.GetUsersAsync(
            realm: options.Value.Realm,
            briefRepresentation: true,
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

    private async Task<bool> KeycloakUserExistsAsync(Guid userId, CancellationToken cancellationToken)
    {
        using var usersApi = keycloakClientFactory.GetUsersApi();

        try
        {
            await usersApi.GetUsersByUserIdAsync(
                realm: options.Value.Realm,
                userId: userId.ToString(),
                cancellationToken: cancellationToken);
            return true;
        }
        catch (ApiException exception) when (exception.ErrorCode == (int)HttpStatusCode.NotFound)
        {
            return false;
        }
    }

    private async Task<bool> DbUserExistsAsync(Guid userId, CancellationToken cancellationToken)
    {
        var dbUser = await dbContext.Users
            .FindAsync(keyValues: [userId], cancellationToken);
        return dbUser is not null;
    }

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
            .FindAsync(keyValues: [userId], cancellationToken);

        if (dbUser is not null)
        {
            return new User { Id = dbUser.Id };
        }

        return null;
    }

    private async Task<UserRepresentation?> FindKeycloakUserAsync(Guid userId, CancellationToken cancellationToken)
    {
        using var usersApi = keycloakClientFactory.GetUsersApi();

        try
        {
            var keycloakUser = await usersApi.GetUsersByUserIdAsync(
                realm: options.Value.Realm,
                userId: userId.ToString(),
                cancellationToken: cancellationToken);
            return keycloakUser;
        }
        catch (ApiException exception) when (exception.ErrorCode == (int)HttpStatusCode.NotFound)
        {
            return null;
        }
    }
}
