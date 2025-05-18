namespace DotnetProjectManagement.ProjectManagement.Data.Models;

using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

[PrimaryKey(nameof(ProjectUpdatedActivityId), nameof(UserId), nameof(New))]
[SuppressMessage("ReSharper", "PropertyCanBeMadeInitOnly.Global")]
public class ProjectUpdatedActivityMember
{
    public required Guid ProjectUpdatedActivityId { get; set; }

    public ProjectUpdatedActivity ProjectUpdatedActivity { get; set; } = null!;

    public required Guid UserId { get; set; }

    public User User { get; set; } = null!;

    public required bool New { get; set; }

    public required ProjectMemberRole Role { get; set; }

    [Timestamp]
    public uint Version { get; set; }
}
