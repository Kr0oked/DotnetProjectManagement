namespace DotnetProjectManagement.Project.WebAPI.Modules.ProjectManagement.Domain;

public class User(UserData userData)
{
    public string Username { get; } = userData.Username;

    public bool IsAdministrator { get; } = userData.IsAdministrator;
}
