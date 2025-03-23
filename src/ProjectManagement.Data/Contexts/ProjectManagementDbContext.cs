namespace DotnetProjectManagement.ProjectManagement.Data.Contexts;

using Microsoft.EntityFrameworkCore;
using Models;

public class ProjectManagementDbContext(DbContextOptions<ProjectManagementDbContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Project> Projects { get; set; } = null!;
    public DbSet<ProjectMember> ProjectMembers { get; set; } = null!;
    public DbSet<Activity> Activities { get; set; } = null!;
    public DbSet<ProjectActivity> ProjectActivities { get; set; } = null!;
    public DbSet<ProjectCreatedActivity> ProjectCreatedActivities { get; set; } = null!;
    public DbSet<ProjectCreatedActivityMember> ProjectCreatedActivityMembers { get; set; } = null!;
    public DbSet<ProjectUpdatedActivity> ProjectUpdatedActivities { get; set; } = null!;
    public DbSet<ProjectUpdatedActivityMember> ProjectUpdatedActivityMembers { get; set; } = null!;
    public DbSet<ProjectArchivedActivity> ProjectArchivedActivities { get; set; } = null!;
    public DbSet<ProjectRestoredActivity> ProjectRestoredActivities { get; set; } = null!;
}
