namespace DotnetProjectManagement.ProjectManagement.Domain.Entities;

using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;

public class ProjectUpdatedActivity : ProjectActivity
{
    [Required(AllowEmptyStrings = false)]
    [StringLength(255)]
    public required string NewDisplayName { get; init; }

    [Required(AllowEmptyStrings = false)]
    [StringLength(255)]
    public required string OldDisplayName { get; init; }

    public required ImmutableDictionary<Guid, ProjectMemberRole> NewMembers { get; init; }

    public required ImmutableDictionary<Guid, ProjectMemberRole> OldMembers { get; init; }
}
