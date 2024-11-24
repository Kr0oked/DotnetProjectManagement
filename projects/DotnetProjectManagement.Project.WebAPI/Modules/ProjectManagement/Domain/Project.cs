namespace DotnetProjectManagement.Project.WebAPI.Modules.ProjectManagement.Domain;

using System.Collections.ObjectModel;

public class Project(IProjectRepository projectRepository, ProjectData projectData)
{
    public string Id { get; } = projectData.Id;
    public string DisplayName { get; private set; } = projectData.DisplayName;

    public IReadOnlyDictionary<User, ProjectMemberRole> Members => this.GetMembers();

    private Dictionary<User, ProjectMemberRole>? internalMembers;

    public void UpdateDisplayName(string displayName)
    {
        projectRepository.UpdateProjectDisplayName(this.Id, displayName);
        this.DisplayName = displayName;
    }

    public void UpdateMembers(IDictionary<User, ProjectMemberRole> members)
    {
        projectRepository.UpdateMembers(this.Id, members);

        this.internalMembers ??= [];
        this.internalMembers.Clear();

        foreach (var (user, role) in members)
        {
            this.internalMembers.Add(user, role);
        }
    }

    private ReadOnlyDictionary<User, ProjectMemberRole> GetMembers()
    {
        this.internalMembers ??= projectRepository.GetMembers(this.Id).ToDictionary();
        return this.internalMembers.AsReadOnly();
    }
}
