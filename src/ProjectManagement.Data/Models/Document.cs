namespace DotnetProjectManagement.ProjectManagement.Data.Models;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Document
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required(AllowEmptyStrings = false)]
    [StringLength(255)]
    public required string DisplayName { get; set; }

    public required DateTimeOffset CreatedTime { get; set; }

    public required Guid CreatedByUserId { get; set; }

    public User CreatedByUser { get; set; } = null!;

    [Timestamp]
    public uint Version { get; set; }
}
