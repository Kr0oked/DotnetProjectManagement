namespace DotnetProjectManagement.ProjectManagement.Data.Models;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using Domain;
using Domain.Actions;

[SuppressMessage("ReSharper", "PropertyCanBeMadeInitOnly.Global")]
public class ProjectTask
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

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

    public required TaskAction Action { get; set; }

    public required Guid ActorId { get; set; }

    public User Actor { get; set; } = null!;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Timestamp]
    public byte[] Version { get; set; } = null!;
}
