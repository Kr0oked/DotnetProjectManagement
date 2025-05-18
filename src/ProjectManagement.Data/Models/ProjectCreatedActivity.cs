namespace DotnetProjectManagement.ProjectManagement.Data.Models;

using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

[SuppressMessage("ReSharper", "PropertyCanBeMadeInitOnly.Global")]
public class ProjectCreatedActivity : ProjectActivity
{
    [Required(AllowEmptyStrings = false)]
    [StringLength(255)]
    public required string DisplayName { get; set; }

    public ICollection<ProjectCreatedActivityMember> Members { get; } = [];
}
