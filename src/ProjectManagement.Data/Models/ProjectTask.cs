namespace DotnetProjectManagement.ProjectManagement.Data.Models;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using Domain;

[Table("Tasks")]
[SuppressMessage("ReSharper", "PropertyCanBeMadeInitOnly.Global")]
public class ProjectTask
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required(AllowEmptyStrings = false)]
    [StringLength(Constants.Text.DisplayNameMaxLength)]
    public required string DisplayName { get; set; }

    [Required(AllowEmptyStrings = false)]
    [StringLength(Constants.Text.DescriptionMaxLength)]
    public required string Description { get; set; }

    public required bool Open { get; set; }

    public required Guid ProjectId { get; set; }

    public Project Project { get; set; } = null!;

    public ICollection<User> Assignees { get; } = [];

    public ICollection<TaskActivity> Activities { get; } = [];

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    [Timestamp]
    public uint Version { get; set; }
}
