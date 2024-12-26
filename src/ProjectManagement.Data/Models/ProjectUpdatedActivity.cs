namespace DotnetProjectManagement.ProjectManagement.Data.Models;

using System.ComponentModel.DataAnnotations;

public class ProjectUpdatedActivity : ProjectActivity
{
    [Required(AllowEmptyStrings = false)]
    [StringLength(255)]
    public required string NewDisplayName { get; set; }

    [Required(AllowEmptyStrings = false)]
    [StringLength(255)]
    public required string OldDisplayName { get; set; }

    public ICollection<ProjectUpdatedActivityMember> Members { get; } = [];
}
