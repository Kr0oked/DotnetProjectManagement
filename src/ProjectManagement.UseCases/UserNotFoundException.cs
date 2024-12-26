namespace DotnetProjectManagement.ProjectManagement.UseCases;

public class UserNotFoundException(Guid userId)
    : Exception($"Could not find user {userId}");
