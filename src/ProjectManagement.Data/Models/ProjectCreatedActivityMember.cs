namespace DotnetProjectManagement.ProjectManagement.Data.Models;

using System.ComponentModel.DataAnnotations;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

[PrimaryKey(nameof(ProjectCreatedActivityId), nameof(UserId))]
public class ProjectCreatedActivityMember
{
    public required Guid ProjectCreatedActivityId { get; set; }

    public ProjectCreatedActivity ProjectCreatedActivity { get; set; } = null!;

    public required Guid UserId { get; set; }

    public User User { get; set; } = null!;

    public required ProjectMemberRole Role { get; set; }

    [Timestamp]
    public uint Version { get; set; }
}
