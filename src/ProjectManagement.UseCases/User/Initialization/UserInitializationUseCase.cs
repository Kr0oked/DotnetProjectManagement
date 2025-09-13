namespace DotnetProjectManagement.ProjectManagement.UseCases.User.Initialization;

using System.ComponentModel.DataAnnotations;
using Abstractions;
using Domain.Entities;
using DTOs;
using Microsoft.Extensions.Logging;

public class UserInitializationUseCase(IUserRepository userRepository, ILogger<UserInitializationUseCase> logger)
{
    public async Task InitializeUserAsync(
        Actor actor,
        CancellationToken cancellationToken = default)
    {
        var exists = await userRepository.ExistsAsync(actor.UserId, cancellationToken);

        if (exists)
        {
            logger.LogExistingUserFound(actor.UserId);
        }
        else
        {
            var user = await this.CreateUserAsync(actor, cancellationToken);
            logger.LogInitializedUser(user);
        }
    }

    private async Task<User> CreateUserAsync(Actor actor, CancellationToken cancellationToken)
    {
        var user = new User
        {
            Id = actor.UserId,
            FirstName = actor.FirstName,
            LastName = actor.LastName,
        };

        Validator.ValidateObject(user, new ValidationContext(user), true);

        await userRepository.SaveAsync(user, cancellationToken);

        return user;
    }
}
