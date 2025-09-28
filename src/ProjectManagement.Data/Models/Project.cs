namespace DotnetProjectManagement.ProjectManagement.Data.Models;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using Domain;
using Domain.Actions;

[SuppressMessage("ReSharper", "PropertyCanBeMadeInitOnly.Global")]
public class Project
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    [Required(AllowEmptyStrings = false)]
    [StringLength(Constants.Text.DisplayNameMaxLength)]
    public required string DisplayName { get; set; }

    public required bool Archived { get; set; }

    public ICollection<ProjectMember> Members { get; } = [];

    public ICollection<ProjectTask> Tasks { get; } = [];

    public required ProjectAction Action { get; set; }

    public required Guid ActorId { get; set; }

    public User Actor { get; set; } = null!;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Timestamp]
    public byte[] Version { get; set; } = null!;
}
