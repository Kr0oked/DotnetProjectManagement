namespace DotnetProjectManagement.ProjectManagement.UseCases.Exceptions;

public class ProjectArchivedException(Guid projectId)
    : Exception($"Project {projectId} is archived");
