namespace DotnetProjectManagement.ProjectManagement.Data.Models;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

[SuppressMessage("ReSharper", "PropertyCanBeMadeInitOnly.Global")]
public class User
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public Guid Id { get; set; } = Guid.NewGuid();

    public ICollection<ProjectMember> ProjectMemberships { get; } = [];

    public ICollection<ProjectTask> AssignedTasks { get; } = [];

    public ICollection<Activity> History { get; } = [];

    public ICollection<ProjectCreatedActivityMember> ProjectCreatedActivityMemberships { get; } = [];

    public ICollection<ProjectUpdatedActivityMember> ProjectUpdatedActivityMemberships { get; } = [];

    public ICollection<TaskUpdatedActivity> TaskUpdatedActivityNewAssignees { get; } = [];

    public ICollection<TaskUpdatedActivity> TaskUpdatedActivityOldAssignees { get; } = [];

    [Timestamp]
    public uint Version { get; set; }
}
