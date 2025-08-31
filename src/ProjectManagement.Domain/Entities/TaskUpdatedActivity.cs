namespace DotnetProjectManagement.ProjectManagement.Domain.Entities;

using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;

public class TaskUpdatedActivity : TaskActivity
{
    [Required(AllowEmptyStrings = false)]
    [StringLength(Constants.Text.DisplayNameMaxLength)]
    public required string NewDisplayName { get; init; }

    [Required(AllowEmptyStrings = false)]
    [StringLength(Constants.Text.DisplayNameMaxLength)]
    public required string OldDisplayName { get; init; }

    [Required(AllowEmptyStrings = false)]
    [StringLength(Constants.Text.DescriptionMaxLength)]
    public required string NewDescription { get; init; }

    [Required(AllowEmptyStrings = false)]
    [StringLength(Constants.Text.DescriptionMaxLength)]
    public required string OldDescription { get; init; }

    public required ImmutableHashSet<Guid> NewAssignees { get; init; }

    public required ImmutableHashSet<Guid> OldAssignees { get; init; }
}
