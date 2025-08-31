namespace DotnetProjectManagement.ProjectManagement.Domain.Entities;

using System.ComponentModel.DataAnnotations;
using System.Text;

public class ProjectTask
{
    public Guid Id { get; init; } = Guid.NewGuid();

    [Required(AllowEmptyStrings = false)]
    [StringLength(Constants.Text.DisplayNameMaxLength)]
    public required string DisplayName { get; set; }

    [Required(AllowEmptyStrings = false)]
    [StringLength(Constants.Text.DescriptionMaxLength)]
    public required string Description { get; set; }

    public required bool Open { get; set; }

    public required HashSet<Guid> Assignees { get; set; }

    public required Guid ProjectId { get; init; }

    public override string ToString() =>
        new StringBuilder()
            .Append("Task { ")
            .Append(nameof(this.Id)).Append(" = ").Append(this.Id).Append(", ")
            .Append(nameof(this.DisplayName)).Append(" = ").Append(this.DisplayName).Append(", ")
            .Append(nameof(this.Description)).Append(" = ").Append(this.Description).Append(", ")
            .Append(nameof(this.Open)).Append(" = ").Append(this.Open).Append(", ")
            .Append(nameof(this.Assignees)).Append(" = [ ").Append(this.AssigneesToString()).Append(" ], ")
            .Append(nameof(this.ProjectId)).Append(" = ").Append(this.ProjectId)
            .Append(" }")
            .ToString();

    private string AssigneesToString() => string.Join(", ", this.Assignees.Order());
}
