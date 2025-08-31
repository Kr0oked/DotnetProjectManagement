namespace DotnetProjectManagement.ProjectManagement.Web.Models;

using System.Collections.Immutable;
using System.ComponentModel;

public record TaskRepresentation
{
    [Description("ID of the task.")]
    public required Guid Id { get; init; }

    [Description("Name of the task.")]
    public required string DisplayName { get; init; }

    [Description("Description of the task.")]
    public required string Description { get; init; }

    public required bool Open { get; init; }

    [Description("Set of the user IDs that are assigned to the task.")]
    public required ImmutableHashSet<Guid> Assignees { get; init; }

    public virtual bool Equals(TaskRepresentation? other)
    {
        if (other is null)
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return this.Id.Equals(other.Id) &&
               this.DisplayName == other.DisplayName &&
               this.Description == other.Description &&
               this.Open == other.Open &&
               this.Assignees.SetEquals(other.Assignees);
    }

    public override int GetHashCode() => HashCode.Combine(
        this.Id,
        this.DisplayName,
        this.Description,
        this.Open,
        this.Assignees);
}
