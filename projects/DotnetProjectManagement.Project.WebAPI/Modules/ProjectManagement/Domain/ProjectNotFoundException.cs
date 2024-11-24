namespace DotnetProjectManagement.Project.WebAPI.Modules.ProjectManagement.Domain;

public class ProjectNotFoundException(string projectId)
    : Exception($"Project with ID '{projectId}' was not found.");
