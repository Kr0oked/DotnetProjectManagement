namespace DotnetProjectManagement.ProjectManagement.Data.Contexts;

using Microsoft.EntityFrameworkCore;
using Models;

public class ProjectManagementDbContext(DbContextOptions<ProjectManagementDbContext> options) : DbContext(options)
{
    public const string TemporalPeriodStart = "PeriodStart";

    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Project> Projects { get; set; } = null!;
    public DbSet<ProjectMember> ProjectMembers { get; set; } = null!;
    public DbSet<ProjectTask> Tasks { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().ToTable("Users", b => b.IsTemporal());
        modelBuilder.Entity<Project>().ToTable("Projects", b => b.IsTemporal());
        modelBuilder.Entity<ProjectMember>().ToTable("ProjectMembers", b => b.IsTemporal());
        modelBuilder.Entity<ProjectTask>().ToTable("Tasks", b => b.IsTemporal());

        modelBuilder.Entity<Project>()
            .HasOne(project => project.Actor)
            .WithMany(user => user.ActorForProjects)
            .HasForeignKey(project => project.ActorId)
            .IsRequired()
            .OnDelete(DeleteBehavior.ClientSetNull);

        modelBuilder.Entity<ProjectTask>()
            .HasOne(task => task.Project)
            .WithMany(project => project.Tasks)
            .HasForeignKey(task => task.ProjectId)
            .IsRequired();

        modelBuilder.Entity<ProjectTask>()
            .HasMany(task => task.Assignees)
            .WithMany(user => user.AssignedTasks);

        modelBuilder.Entity<ProjectTask>()
            .HasOne(task => task.Actor)
            .WithMany(user => user.ActorForTasks)
            .HasForeignKey(task => task.ActorId)
            .IsRequired()
            .OnDelete(DeleteBehavior.ClientSetNull);

        modelBuilder.Entity<ProjectMember>()
            .HasOne(member => member.Project)
            .WithMany(project => project.Members)
            .HasForeignKey(member => member.ProjectId)
            .IsRequired();

        modelBuilder.Entity<ProjectMember>()
            .HasOne(member => member.User)
            .WithMany(user => user.ProjectMemberships)
            .HasForeignKey(member => member.UserId)
            .IsRequired();
    }
}
