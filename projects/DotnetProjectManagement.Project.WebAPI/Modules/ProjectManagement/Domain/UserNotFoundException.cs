namespace DotnetProjectManagement.Project.WebAPI.Modules.ProjectManagement.Domain;

public class UserNotFoundException(string username)
    : Exception($"User with username '{username}' was not found.");
