namespace DotnetProjectManagement.Project.WebAPI.Modules.ProjectManagement.Domain;

public interface IUserRepository
{
    public UserData? FindUserByUsername(string username);
}
