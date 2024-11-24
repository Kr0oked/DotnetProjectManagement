namespace DotnetProjectManagement.Project.WebAPI.Modules.ProjectManagement.Domain;

public class UserFactory(IUserRepository userRepository)
{
    public User GetUser(string username)
    {
        var userData = userRepository
            .FindUserByUsername(username) ?? throw new UserNotFoundException(username);

        return new User(userData);
    }
}
