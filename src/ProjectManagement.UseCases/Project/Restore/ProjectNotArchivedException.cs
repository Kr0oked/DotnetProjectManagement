namespace DotnetProjectManagement.ProjectManagement.UseCases.Project.Restore;

public class ProjectNotArchivedException(Guid projectId)
    : Exception($"Project {projectId} is not archived");
