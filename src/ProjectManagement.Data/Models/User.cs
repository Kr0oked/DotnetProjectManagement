namespace DotnetProjectManagement.ProjectManagement.Data.Models;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class User
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public Guid Id { get; set; } = Guid.NewGuid();

    public ICollection<ProjectMember> ProjectMemberships { get; } = [];

    public ICollection<ProjectCreatedActivityMember> ProjectCreatedActivityMemberships { get; } = [];

    public ICollection<ProjectUpdatedActivityMember> ProjectUpdatedActivityMemberships { get; } = [];

    public ICollection<Activity> Activities { get; } = [];

    public ICollection<Document> Documents { get; } = [];

    [Timestamp]
    public uint Version { get; set; }
}
