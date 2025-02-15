namespace DotnetProjectManagement.ProjectManagement.UseCases.Exceptions;

public class ProjectNotFoundException(Guid projectId)
    : Exception($"Could not find project {projectId}");
