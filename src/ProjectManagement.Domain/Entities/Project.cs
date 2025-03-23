namespace DotnetProjectManagement.ProjectManagement.Domain.Entities;

using System.ComponentModel.DataAnnotations;
using System.Text;

public class Project : IValidatableObject
{
    public Guid Id { get; init; } = Guid.NewGuid();

    [Required(AllowEmptyStrings = false)]
    [StringLength(255)]
    public required string DisplayName { get; set; }

    public required bool Archived { get; set; }

    public required Dictionary<Guid, ProjectMemberRole> Members { get; set; }

    public override string ToString() =>
        new StringBuilder()
            .Append(nameof(Project))
            .Append(" { ")
            .Append(nameof(this.Id)).Append(" = ").Append(this.Id).Append(", ")
            .Append(nameof(this.DisplayName)).Append(" = ").Append(this.DisplayName).Append(", ")
            .Append(nameof(this.Archived)).Append(" = ").Append(this.Archived).Append(", ")
            .Append(nameof(this.Members)).Append(" = { ").Append(this.MembersToString()).Append(" }")
            .Append(" }")
            .ToString();

    private string MembersToString() => string.Join(", ", this.Members);

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext) =>
        this.Members
            .Where(member => !Enum.IsDefined(member.Value))
            .Select(member =>
                new ValidationResult($"The value {member.Value} is not a valid role.", [nameof(this.Members)]));

    public ProjectMemberRole? GetRoleOfUser(Guid userId) =>
        this.Members.TryGetValue(userId, out var role) ? role : null;
}
