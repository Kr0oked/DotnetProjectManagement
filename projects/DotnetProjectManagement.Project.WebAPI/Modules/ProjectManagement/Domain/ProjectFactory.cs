namespace DotnetProjectManagement.Project.WebAPI.Modules.ProjectManagement.Domain;

public class ProjectFactory(IProjectRepository projectRepository)
{
    public Project GetProject(string projectId)
    {
        var projectData = projectRepository
            .FindProjectById(projectId) ?? throw new ProjectNotFoundException(projectId);

        return new Project(projectRepository, projectData);
    }

    public Project CreateProject(string displayName)
    {
        var projectData = projectRepository.CreatProject(displayName);
        return new Project(projectRepository, projectData);
    }
}
