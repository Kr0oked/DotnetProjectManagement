namespace DotnetProjectManagement.ProjectManagement.Data.Models;

using System.Diagnostics.CodeAnalysis;

[SuppressMessage("ReSharper", "PropertyCanBeMadeInitOnly.Global")]
public abstract class TaskActivity : Activity
{
    public required Guid TaskId { get; set; }

    public ProjectTask Task { get; set; } = null!;
}
