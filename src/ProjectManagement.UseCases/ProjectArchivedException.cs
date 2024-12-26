namespace DotnetProjectManagement.ProjectManagement.UseCases;

public class ProjectArchivedException(Guid projectId)
    : Exception($"Project {projectId} is archived");
