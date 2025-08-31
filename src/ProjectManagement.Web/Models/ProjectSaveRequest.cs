namespace DotnetProjectManagement.ProjectManagement.Web.Models;

using System.Collections.Immutable;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Domain;
using Domain.Entities;

public record ProjectSaveRequest : IValidatableObject
{
    [Description("Name of the project.")]
    [Required(AllowEmptyStrings = false)]
    [StringLength(Constants.Text.DisplayNameMaxLength)]
    public required string DisplayName { get; init; }

    [Description("Dictionary of project members. Maps the user ID to the corresponding role.")]
    public required ImmutableDictionary<Guid, ProjectMemberRole> Members { get; init; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext) =>
        this.Members
            .Where(member => !Enum.IsDefined(member.Value))
            .Select(member =>
                new ValidationResult($"The value {member.Value} is not a valid role.", [nameof(this.Members)]));
}
