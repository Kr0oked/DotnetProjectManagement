namespace DotnetProjectManagement.ProjectManagement.UseCases.Exceptions;

using DTOs;

public class ManagerRequiredException(Actor actor, Guid projectId)
    : Exception($"User {actor.UserId} is not a manager in project {projectId}");
