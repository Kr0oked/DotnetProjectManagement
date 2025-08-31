namespace DotnetProjectManagement.ProjectManagement.UseCases.User.GetDetails;

using Abstractions;
using Domain.Entities;
using DTOs;
using Exceptions;
using Extensions;

public class UserGetDetailsUseCase(IUserRepository userRepository)
{
    public async Task<UserDto> GetUserDetailsAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var user = await this.GetUser(userId, cancellationToken);
        return user.ToDto();
    }

    private async Task<User> GetUser(Guid userId, CancellationToken cancellationToken) =>
        await userRepository.FindOneAsync(userId, cancellationToken)
        ?? throw new UserNotFoundException(userId);
}
