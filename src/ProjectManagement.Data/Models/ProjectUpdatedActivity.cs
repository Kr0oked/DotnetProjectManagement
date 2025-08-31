namespace DotnetProjectManagement.ProjectManagement.Data.Models;

using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using Domain;

[SuppressMessage("ReSharper", "PropertyCanBeMadeInitOnly.Global")]
public class ProjectUpdatedActivity : ProjectActivity
{
    [Required(AllowEmptyStrings = false)]
    [StringLength(Constants.Text.DisplayNameMaxLength)]
    public required string NewDisplayName { get; set; }

    [Required(AllowEmptyStrings = false)]
    [StringLength(Constants.Text.DisplayNameMaxLength)]
    public required string OldDisplayName { get; set; }

    public ICollection<ProjectUpdatedActivityMember> Members { get; } = [];
}
