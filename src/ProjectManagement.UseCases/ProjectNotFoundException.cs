namespace DotnetProjectManagement.ProjectManagement.UseCases;

public class ProjectNotFoundException(Guid projectId)
    : Exception($"Could not find project {projectId}");
