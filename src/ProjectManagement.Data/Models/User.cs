namespace DotnetProjectManagement.ProjectManagement.Data.Models;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using Domain;

[SuppressMessage("ReSharper", "PropertyCanBeMadeInitOnly.Global")]
public class User
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    [Required(AllowEmptyStrings = false)]
    [StringLength(Constants.Text.DisplayNameMaxLength)]
    public required string FirstName { get; set; }

    [Required(AllowEmptyStrings = false)]
    [StringLength(Constants.Text.DisplayNameMaxLength)]
    public required string LastName { get; set; }

    public ICollection<ProjectMember> ProjectMemberships { get; } = [];

    public ICollection<ProjectTask> AssignedTasks { get; } = [];

    public ICollection<Activity> History { get; } = [];

    public ICollection<ProjectCreatedActivityMember> ProjectCreatedActivityMemberships { get; } = [];

    public ICollection<ProjectUpdatedActivityMember> ProjectUpdatedActivityMemberships { get; } = [];

    public ICollection<TaskUpdatedActivity> TaskUpdatedActivityNewAssignees { get; } = [];

    public ICollection<TaskUpdatedActivity> TaskUpdatedActivityOldAssignees { get; } = [];

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    [Timestamp]
    public uint Version { get; set; }
}
