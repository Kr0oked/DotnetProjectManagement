namespace DotnetProjectManagement.ProjectManagement.UseCases.User.List;

using Abstractions;
using DTOs;
using Extensions;

public class UserListUseCase(IUserRepository userRepository)
{
    public async Task<Page<UserDto>> ListUsersAsync(
        PageRequest pageRequest,
        CancellationToken cancellationToken = default)
    {
        var page = await userRepository.FindAllAsync(pageRequest, cancellationToken);
        return page.Map(user => user.ToDto());
    }
}
