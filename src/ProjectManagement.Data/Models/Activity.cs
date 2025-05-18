namespace DotnetProjectManagement.ProjectManagement.Data.Models;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

[SuppressMessage("ReSharper", "PropertyCanBeMadeInitOnly.Global")]
public abstract class Activity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public Guid Id { get; set; } = Guid.NewGuid();

    public required Guid UserId { get; set; }

    public User User { get; set; } = null!;

    public required DateTimeOffset Timestamp { get; set; }

    [Timestamp]
    public uint Version { get; set; }
}
