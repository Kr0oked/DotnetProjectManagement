namespace DotnetProjectManagement.ProjectManagement.Web.Models;

using System.Collections.Immutable;
using System.ComponentModel;
using Domain.Entities;

public record ProjectRepresentation
{
    [Description("ID of the project.")]
    public required Guid Id { get; init; }

    [Description("Name of the project.")]
    public required string DisplayName { get; init; }

    [Description("Determines if the project is archived.")]
    public required bool Archived { get; init; }

    [Description("Dictionary of project members. Maps the user ID to the corresponding role.")]
    public required ImmutableDictionary<Guid, ProjectMemberRole> Members { get; init; }

    public virtual bool Equals(ProjectRepresentation? other)
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
               this.Archived == other.Archived &&
               this.Members.Count == other.Members.Count &&
               // ReSharper disable once UsageOfDefaultStructEquality
               !this.Members.Except(other.Members).Any();
    }

    public override int GetHashCode() => HashCode.Combine(this.Id, this.DisplayName, this.Archived, this.Members);
}
