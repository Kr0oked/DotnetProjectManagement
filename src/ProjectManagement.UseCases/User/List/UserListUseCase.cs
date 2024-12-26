namespace DotnetProjectManagement.ProjectManagement.UseCases.User.List;

using Abstractions;
using DTOs;
using Mappers;

public class UserListUseCase(IUserRepository userRepository)
{
    public async Task<Page<UserDto>> ListUsersAsync(
        Actor actor,
        PageRequest pageRequest,
        CancellationToken cancellationToken = default)
    {
        var page = await userRepository.FindAllAsync(actor.UserId, pageRequest, cancellationToken);
        return page.Map(user => user.ToDto());
    }
}
