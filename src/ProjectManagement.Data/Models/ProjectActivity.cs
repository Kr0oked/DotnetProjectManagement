namespace DotnetProjectManagement.ProjectManagement.Data.Models;

public abstract class ProjectActivity : Activity
{
    public required Guid ProjectId { get; set; }

    public Project Project { get; set; } = null!;
}
