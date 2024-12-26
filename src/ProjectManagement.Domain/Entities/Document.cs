namespace DotnetProjectManagement.ProjectManagement.Domain.Entities;

using System.ComponentModel.DataAnnotations;

public class Document
{
    public Guid Id { get; init; } = Guid.NewGuid();

    [Required(AllowEmptyStrings = false)]
    [StringLength(255)]
    public required string DisplayName { get; init; }

    public required DateTimeOffset CreatedTime { get; init; }

    public required Guid CreatedByUserId { get; init; }
}
