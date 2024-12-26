namespace DotnetProjectManagement.ProjectManagement.UseCases.User.GetDetails;

using Abstractions;
using DTOs;
using Mappers;

public class UserGetDetailsUseCase(IUserRepository userRepository)
{
    public async Task<UserDto> GetUserDetailsAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var user = await userRepository.FindOneAsync(userId, cancellationToken)
                   ?? throw new UserNotFoundException(userId);

        return user.ToDto();
    }
}
