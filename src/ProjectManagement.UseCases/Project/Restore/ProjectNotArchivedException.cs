namespace DotnetProjectManagement.ProjectManagement.UseCases.Project.Restore;

public class ProjectNotArchivedException(Guid projectId)
    : Exception($"Project {projectId} not archived");
