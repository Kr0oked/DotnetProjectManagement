namespace DotnetProjectManagement.ProjectManagement.UseCases;

public class ProjectMemberException(Actor actor, Guid projectId)
    : Exception($"User {actor.UserId} is not a member of project {projectId}");
