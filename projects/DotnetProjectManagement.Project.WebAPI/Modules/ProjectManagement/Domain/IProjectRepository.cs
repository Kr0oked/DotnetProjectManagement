namespace DotnetProjectManagement.Project.WebAPI.Modules.ProjectManagement.Domain;

public interface IProjectRepository
{
    public ProjectData? FindProjectById(string projectId);

    public ProjectData CreatProject(string displayName);

    public void UpdateProjectDisplayName(string projectId, string displayName);

    public IDictionary<User, ProjectMemberRole> GetMembers(string projectId);

    public void UpdateMembers(string projectId, IDictionary<User, ProjectMemberRole> members);
}
