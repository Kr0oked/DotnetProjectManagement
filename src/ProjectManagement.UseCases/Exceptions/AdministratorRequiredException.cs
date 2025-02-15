namespace DotnetProjectManagement.ProjectManagement.UseCases.Exceptions;

using DTOs;

public class AdministratorRequiredException(Actor actor)
    : Exception($"User {actor.UserId} is not an administrator");
