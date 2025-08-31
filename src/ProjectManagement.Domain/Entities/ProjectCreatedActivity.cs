namespace DotnetProjectManagement.ProjectManagement.Domain.Entities;

using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;

public class ProjectCreatedActivity : ProjectActivity
{
    [Required(AllowEmptyStrings = false)]
    [StringLength(Constants.Text.DisplayNameMaxLength)]
    public required string DisplayName { get; init; }

    public required ImmutableDictionary<Guid, ProjectMemberRole> Members { get; init; }
}
