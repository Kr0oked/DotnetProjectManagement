namespace DotnetProjectManagement.ProjectManagement.Domain.Entities;

using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;

public class TaskCreatedActivity : TaskActivity
{
    [Required(AllowEmptyStrings = false)]
    [StringLength(Constants.Text.DisplayNameMaxLength)]
    public required string DisplayName { get; init; }

    [Required(AllowEmptyStrings = false)]
    [StringLength(Constants.Text.DescriptionMaxLength)]
    public required string Description { get; init; }

    public required ImmutableHashSet<Guid> Assignees { get; init; }
}
