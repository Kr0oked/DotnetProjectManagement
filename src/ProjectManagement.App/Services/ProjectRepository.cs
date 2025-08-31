namespace DotnetProjectManagement.ProjectManagement.App.Services;

using System.Collections.Immutable;
using Data.Contexts;
using Data.Models;
using Microsoft.EntityFrameworkCore;
using UseCases.Abstractions;
using UseCases.DTOs;
using UseCases.Exceptions;
using ProjectEntity = Domain.Entities.Project;
using ProjectDb = Data.Models.Project;

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
            .FirstOrDefaultAsync(cancellationToken);

    public async Task<ProjectEntity> SaveAsync(ProjectEntity project, CancellationToken cancellationToken = default)
    {
        var projectDb = project.Id == Guid.Empty
            ? await this.CreateProjectAsync(project, cancellationToken)
            : await this.UpdateProjectAsync(project, cancellationToken);

        await dbContext.SaveChangesAsync(cancellationToken);

        return MapToEntity(projectDb);
    }

    private async Task<ProjectDb> CreateProjectAsync(ProjectEntity project, CancellationToken cancellationToken)
    {
        var projectDb = new ProjectDb
        {
            Id = project.Id,
            DisplayName = project.DisplayName,
            Archived = project.Archived
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

    private async Task<ProjectDb> UpdateProjectAsync(ProjectEntity project, CancellationToken cancellationToken)
    {
        var existingProject = await dbContext.Projects
            .FindAsync([project.Id], cancellationToken) ?? throw new ProjectNotFoundException(project.Id);

        existingProject.DisplayName = project.DisplayName;
        existingProject.Archived = project.Archived;

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

    private async Task<User> GetUserAsync(Guid userId, CancellationToken cancellationToken)
    {
        var existingUser = await dbContext.Users.FindAsync([userId], cancellationToken);

        if (existingUser is not null)
        {
            return existingUser;
        }

        var user = new User { Id = userId };
        dbContext.Users.Add(user);
        return user;
    }

    private static ProjectEntity MapToEntity(ProjectDb project) => new()
    {
        Id = project.Id,
        DisplayName = project.DisplayName,
        Archived = project.Archived,
        Members = project.Members.ToDictionary(member => member.UserId, member => member.Role)
    };
}
