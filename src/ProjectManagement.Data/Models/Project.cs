namespace DotnetProjectManagement.ProjectManagement.Data.Models;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

[SuppressMessage("ReSharper", "PropertyCanBeMadeInitOnly.Global")]
public class Project
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required(AllowEmptyStrings = false)]
    [StringLength(255)]
    public required string DisplayName { get; set; }

    public required bool Archived { get; set; }

    public ICollection<ProjectMember> Members { get; } = [];

    [Timestamp]
    public uint Version { get; set; }
}
