namespace DotnetProjectManagement.ProjectManagement.App.Services;

using System.Collections.Immutable;
using Data.Contexts;
using Data.Models;
using Domain.Actions;
using Microsoft.EntityFrameworkCore;
using UseCases.Abstractions;
using UseCases.DTOs;
using UseCases.Exceptions;
using ProjectEntity = Domain.Entities.Project;
using ProjectDb = Data.Models.Project;
using User = Data.Models.User;

public class ProjectRepository(ProjectManagementDbContext dbContext) : IProjectRepository
{
    public async Task<Page<ProjectEntity>> FindAllAsync(
        PageRequest pageRequest,
        CancellationToken cancellationToken = default)
    {
        var queryable = dbContext.Projects
            .OrderBy(project => project.CreatedAt);

        var totalElements = await queryable.LongCountAsync(cancellationToken);

        var projects = queryable
            .Skip(pageRequest.Offset)
            .Take(pageRequest.Size)
            .Include(project => project.Members)
            .Select(project => MapToEntity(project))
            .ToImmutableList();

        return new Page<ProjectEntity>(projects, pageRequest, totalElements);
    }

    public async Task<Page<ProjectEntity>> FindByNotArchivedAndMembershipAsync(
        Guid userId,
        PageRequest pageRequest,
        CancellationToken cancellationToken = default)
    {
        var queryable = dbContext.Projects
            .Where(project => !project.Archived)
            .Where(project => project.Members.Any(member => member.UserId == userId))
            .OrderBy(project => project.CreatedAt);

        var totalElements = await queryable.LongCountAsync(cancellationToken);

        var projects = queryable
            .Skip(pageRequest.Offset)
            .Take(pageRequest.Size)
            .Include(project => project.Members)
            .Select(project => MapToEntity(project))
            .ToImmutableList();

        return new Page<ProjectEntity>(projects, pageRequest, totalElements);
    }

    public async Task<ProjectEntity?> FindOneAsync(Guid projectId, CancellationToken cancellationToken = default) =>
        await dbContext.Projects
            .Where(project => project.Id == projectId)
            .Include(project => project.Members)
            .Select(project => MapToEntity(project))
            .SingleOrDefaultAsync(cancellationToken);

    public async Task<List<HistoryEntry<ProjectAction, ProjectEntity>>> GetHistory(
        Guid projectId,
        CancellationToken cancellationToken = default)
    {
        var instants = await dbContext.Projects
            .TemporalAll()
            .Where(project => project.Id == projectId)
            .OrderBy(project => EF.Property<DateTime>(project, ProjectManagementDbContext.TemporalPeriodStart))
            .Select(project => EF.Property<DateTime>(project, ProjectManagementDbContext.TemporalPeriodStart))
            .ToListAsync(cancellationToken);

        var historyEntries = new List<HistoryEntry<ProjectAction, ProjectEntity>>();
        foreach (var instant in instants)
        {
            var historyEntry = await this.GetHistoryEntry(projectId, instant, cancellationToken);
            historyEntries.Add(historyEntry);
        }

        return historyEntries;
    }

    private async Task<HistoryEntry<ProjectAction, ProjectEntity>> GetHistoryEntry(
        Guid projectId,
        DateTime instant,
        CancellationToken cancellationToken) =>
        await dbContext.Projects.TemporalAsOf(instant)
            .Where(project => project.Id == projectId)
            .Include(project => project.Members)
            .Join(
                dbContext.Users,
                project => project.ActorId,
                user => user.Id,
                (project, user) => new
                {
                    project,
                    user
                }
            )
            .Select(entry => new HistoryEntry<ProjectAction, ProjectEntity>
            {
                Action = entry.project.Action,
                Entity = MapToEntity(entry.project),
                Timestamp = EF.Property<DateTime>(entry.project, ProjectManagementDbContext.TemporalPeriodStart),
                User = new UserDto
                {
                    Id = entry.user.Id,
                    FirstName = entry.user.FirstName,
                    LastName = entry.user.LastName
                }
            })
            .SingleAsync(cancellationToken);

    public async Task<ProjectEntity> SaveAsync(
        ProjectEntity project,
        ProjectAction action,
        Guid actorUserId,
        CancellationToken cancellationToken = default)
    {
        var projectDb = project.Id == Guid.Empty
            ? await this.CreateProjectAsync(project, action, actorUserId, cancellationToken)
            : await this.UpdateProjectAsync(project, action, actorUserId, cancellationToken);

        await dbContext.SaveChangesAsync(cancellationToken);

        return MapToEntity(projectDb);
    }

    private async Task<ProjectDb> CreateProjectAsync(
        ProjectEntity project,
        ProjectAction action,
        Guid actorUserId,
        CancellationToken cancellationToken)
    {
        var projectDb = new ProjectDb
        {
            Id = project.Id,
            DisplayName = project.DisplayName,
            Archived = project.Archived,
            Action = action,
            ActorId = actorUserId
        };

        foreach (var (memberUserId, memberRole) in project.Members)
        {
            projectDb.Members.Add(new ProjectMember
            {
                ProjectId = projectDb.Id,
                Project = projectDb,
                UserId = memberUserId,
                User = await this.GetUserAsync(memberUserId, cancellationToken),
                Role = memberRole
            });
        }

        dbContext.Projects.Add(projectDb);

        return projectDb;
    }

    private async Task<ProjectDb> UpdateProjectAsync(
        ProjectEntity project,
        ProjectAction action,
        Guid actorUserId,
        CancellationToken cancellationToken)
    {
        var existingProject = await dbContext.Projects
            .FindAsync([project.Id], cancellationToken) ?? throw new ProjectNotFoundException(project.Id);

        existingProject.DisplayName = project.DisplayName;
        existingProject.Archived = project.Archived;
        existingProject.Action = action;
        existingProject.ActorId = actorUserId;

        await dbContext.Entry(existingProject)
            .Collection(p => p.Members)
            .LoadAsync(cancellationToken);

        var obsoleteMembers = existingProject.Members
            .Where(member => project.Members.All(entry => entry.Key != member.UserId))
            .ToImmutableList();
        dbContext.ProjectMembers.RemoveRange(obsoleteMembers);

        foreach (var (memberUserId, memberRole) in project.Members)
        {
            var existingMember = existingProject.Members.FirstOrDefault(member => member.UserId == memberUserId);

            if (existingMember is not null)
            {
                existingMember.Role = memberRole;
            }
            else
            {
                existingProject.Members.Add(new ProjectMember
                {
                    ProjectId = existingProject.Id,
                    Project = existingProject,
                    UserId = memberUserId,
                    User = await this.GetUserAsync(memberUserId, cancellationToken),
                    Role = memberRole
                });
            }
        }

        return existingProject;
    }

    private async Task<User> GetUserAsync(Guid userId, CancellationToken cancellationToken) =>
        await dbContext.Users.FindAsync([userId], cancellationToken) ?? throw new UserNotFoundException(userId);

    private static ProjectEntity MapToEntity(ProjectDb project) => new()
    {
        Id = project.Id,
        DisplayName = project.DisplayName,
        Archived = project.Archived,
        Members = project.Members.ToDictionary(member => member.UserId, member => member.Role)
    };
}
