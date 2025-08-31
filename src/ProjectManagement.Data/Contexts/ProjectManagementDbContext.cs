namespace DotnetProjectManagement.ProjectManagement.Data.Contexts;

using Microsoft.EntityFrameworkCore;
using Models;

public class ProjectManagementDbContext(DbContextOptions<ProjectManagementDbContext> options) : DbContext(options)
{
    public DbSet<Activity> Activities { get; set; } = null!;

    public DbSet<User> Users { get; set; } = null!;

    public DbSet<Project> Projects { get; set; } = null!;
    public DbSet<ProjectMember> ProjectMembers { get; set; } = null!;
    public DbSet<ProjectActivity> ProjectActivities { get; set; } = null!;
    public DbSet<ProjectCreatedActivity> ProjectCreatedActivities { get; set; } = null!;
    public DbSet<ProjectCreatedActivityMember> ProjectCreatedActivityMembers { get; set; } = null!;
    public DbSet<ProjectUpdatedActivity> ProjectUpdatedActivities { get; set; } = null!;
    public DbSet<ProjectUpdatedActivityMember> ProjectUpdatedActivityMembers { get; set; } = null!;
    public DbSet<ProjectArchivedActivity> ProjectArchivedActivities { get; set; } = null!;
    public DbSet<ProjectRestoredActivity> ProjectRestoredActivities { get; set; } = null!;

    public DbSet<ProjectTask> Tasks { get; set; } = null!;
    public DbSet<TaskActivity> TaskActivities { get; set; } = null!;
    public DbSet<TaskCreatedActivity> TaskCreatedActivities { get; set; } = null!;
    public DbSet<TaskUpdatedActivity> TaskUpdatedActivities { get; set; } = null!;
    public DbSet<TaskClosedActivity> TaskClosedActivities { get; set; } = null!;
    public DbSet<TaskReopenedActivity> TaskReopenedActivities { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TaskUpdatedActivity>()
            .HasMany(e => e.NewAssignees)
            .WithMany(e => e.TaskUpdatedActivityNewAssignees);

        modelBuilder.Entity<TaskUpdatedActivity>()
            .HasMany(e => e.OldAssignees)
            .WithMany(e => e.TaskUpdatedActivityOldAssignees);
    }
}
