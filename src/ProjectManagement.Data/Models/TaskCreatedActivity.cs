namespace DotnetProjectManagement.ProjectManagement.Data.Models;

using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using Domain;

[SuppressMessage("ReSharper", "PropertyCanBeMadeInitOnly.Global")]
public class TaskCreatedActivity : TaskActivity
{
    [Required(AllowEmptyStrings = false)]
    [StringLength(Constants.Text.DisplayNameMaxLength)]
    public required string DisplayName { get; set; }

    [Required(AllowEmptyStrings = false)]
    [StringLength(Constants.Text.DescriptionMaxLength)]
    public required string Description { get; set; }

    public ICollection<User> Assignees { get; } = [];
}
