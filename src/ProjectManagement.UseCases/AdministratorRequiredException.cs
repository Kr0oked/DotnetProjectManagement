namespace DotnetProjectManagement.ProjectManagement.UseCases;

public class AdministratorRequiredException(Actor actor)
    : Exception($"User {actor.UserId} is not an administrator");
