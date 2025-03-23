namespace DotnetProjectManagement.ProjectManagement.UseCases.Project.Archive;

public class ProjectAlreadyArchivedException(Guid projectId)
    : Exception($"Project {projectId} is already archived");
