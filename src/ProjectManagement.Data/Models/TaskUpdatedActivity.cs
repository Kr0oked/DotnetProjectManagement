namespace DotnetProjectManagement.ProjectManagement.Data.Models;

using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using Domain;

[SuppressMessage("ReSharper", "PropertyCanBeMadeInitOnly.Global")]
public class TaskUpdatedActivity : TaskActivity
{
    [Required(AllowEmptyStrings = false)]
    [StringLength(Constants.Text.DisplayNameMaxLength)]
    public required string NewDisplayName { get; set; }

    [Required(AllowEmptyStrings = false)]
    [StringLength(Constants.Text.DisplayNameMaxLength)]
    public required string OldDisplayName { get; set; }

    [Required(AllowEmptyStrings = false)]
    [StringLength(Constants.Text.DescriptionMaxLength)]
    public required string NewDescription { get; set; }

    [Required(AllowEmptyStrings = false)]
    [StringLength(Constants.Text.DescriptionMaxLength)]
    public required string OldDescription { get; set; }

    public ICollection<User> NewAssignees { get; } = [];

    public ICollection<User> OldAssignees { get; } = [];
}
