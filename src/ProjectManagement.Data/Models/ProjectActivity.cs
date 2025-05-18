namespace DotnetProjectManagement.ProjectManagement.Data.Models;

using System.Diagnostics.CodeAnalysis;

[SuppressMessage("ReSharper", "PropertyCanBeMadeInitOnly.Global")]
public abstract class ProjectActivity : Activity
{
    public required Guid ProjectId { get; set; }

    public Project Project { get; set; } = null!;
}
