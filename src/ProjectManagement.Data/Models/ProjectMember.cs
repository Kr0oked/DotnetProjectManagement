namespace DotnetProjectManagement.ProjectManagement.Data.Models;

using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

[PrimaryKey(nameof(ProjectId), nameof(UserId))]
[SuppressMessage("ReSharper", "PropertyCanBeMadeInitOnly.Global")]
public class ProjectMember
{
    public required Guid ProjectId { get; set; }

    public Project Project { get; set; } = null!;

    public required Guid UserId { get; set; }

    public User User { get; set; } = null!;

    public required ProjectMemberRole Role { get; set; }

    [Timestamp]
    public uint Version { get; set; }
}
