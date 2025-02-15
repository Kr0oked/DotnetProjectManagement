namespace DotnetProjectManagement.ProjectManagement.UseCases.Exceptions;

public class UserNotFoundException(Guid userId)
    : Exception($"Could not find user {userId}");
