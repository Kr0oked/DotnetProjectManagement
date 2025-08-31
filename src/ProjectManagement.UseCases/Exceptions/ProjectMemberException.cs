namespace DotnetProjectManagement.ProjectManagement.UseCases.Exceptions;

public class ProjectMemberException(Guid userId, Guid projectId)
    : Exception($"User {userId} is not a member of project {projectId}");
