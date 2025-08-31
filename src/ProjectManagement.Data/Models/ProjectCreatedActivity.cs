namespace DotnetProjectManagement.ProjectManagement.Data.Models;

using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using Domain;

[SuppressMessage("ReSharper", "PropertyCanBeMadeInitOnly.Global")]
public class ProjectCreatedActivity : ProjectActivity
{
    [Required(AllowEmptyStrings = false)]
    [StringLength(Constants.Text.DisplayNameMaxLength)]
    public required string DisplayName { get; set; }

    public ICollection<ProjectCreatedActivityMember> Members { get; } = [];
}
