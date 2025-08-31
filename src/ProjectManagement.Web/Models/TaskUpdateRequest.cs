namespace DotnetProjectManagement.ProjectManagement.Web.Models;

using System.Collections.Immutable;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Domain;

public record TaskUpdateRequest
{
    [Description("Name of the project.")]
    [Required(AllowEmptyStrings = false)]
    [StringLength(Constants.Text.DisplayNameMaxLength)]
    public required string DisplayName { get; init; }

    [Description("Description text of the project.")]
    [Required(AllowEmptyStrings = false)]
    [StringLength(Constants.Text.DescriptionMaxLength)]
    public required string Description { get; init; }

    public required ImmutableHashSet<Guid> Assignees { get; init; }
}
