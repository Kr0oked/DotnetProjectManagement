namespace DotnetProjectManagement.ProjectManagement.App.Services;

using System.Collections.Immutable;
using Data.Contexts;
using Data.Models;
using Microsoft.EntityFrameworkCore;
using UseCases.Abstractions;
using UseCases.DTOs;
using UseCases.Exceptions;
using TaskEntity = Domain.Entities.ProjectTask;
using TaskDb = Data.Models.ProjectTask;

public class TaskRepository(ProjectManagementDbContext dbContext) : ITaskRepository
{
    public async Task<TaskEntity?> FindOneAsync(Guid taskId, CancellationToken cancellationToken) =>
        await dbContext.Tasks
            .Where(task => task.Id == taskId)
            .Include(task => task.Assignees)
            .Select(task => MapToEntity(task))
            .FirstOrDefaultAsync(cancellationToken);

    public async Task<Page<TaskEntity>> FindAllByProjectIdAsync(
        Guid projectId,
        PageRequest pageRequest,
        CancellationToken cancellationToken = default)
    {
        var queryable = dbContext.Tasks
            .Where(task => task.ProjectId == projectId)
            .OrderBy(task => task.CreatedAt);

        var totalElements = await queryable.LongCountAsync(cancellationToken);

        var tasks = queryable
            .Skip(pageRequest.Offset)
            .Take(pageRequest.Size)
            .Include(task => task.Assignees)
            .Select(task => MapToEntity(task))
            .ToImmutableList();

        return new Page<TaskEntity>(tasks, pageRequest, totalElements);
    }

    public async Task<TaskEntity> SaveAsync(TaskEntity task, CancellationToken cancellationToken)
    {
        var taskDB = task.Id == Guid.Empty
            ? await this.CreateTaskAsync(task, cancellationToken)
            : await this.UpdateTaskAsync(task, cancellationToken);

        await dbContext.SaveChangesAsync(cancellationToken);

        return MapToEntity(taskDB);
    }

    private async Task<TaskDb> UpdateTaskAsync(TaskEntity task, CancellationToken cancellationToken)
    {
        var existingTask = await dbContext.Tasks
            .FindAsync([task.Id], cancellationToken) ?? throw new TaskNotFoundException(task.Id);

        existingTask.DisplayName = task.DisplayName;
        existingTask.Description = task.Description;
        existingTask.Open = task.Open;

        await dbContext.Entry(existingTask)
            .Collection(t => t.Assignees)
            .LoadAsync(cancellationToken);

        var obsoleteAssignees = existingTask.Assignees
            .Where(assignee => !task.Assignees.Contains(assignee.Id))
            .ToImmutableList();
        foreach (var obsoleteAssignee in obsoleteAssignees)
        {
            existingTask.Assignees.Remove(obsoleteAssignee);
        }

        foreach (var assigneeUserId in task.Assignees)
        {
            var assigneePresent = existingTask.Assignees.Any(assignee => assignee.Id == assigneeUserId);
            if (assigneePresent)
            {
                continue;
            }

            var userAsync = await this.GetUserAsync(assigneeUserId, cancellationToken);
            existingTask.Assignees.Add(userAsync);
        }

        return existingTask;
    }

    private async Task<TaskDb> CreateTaskAsync(TaskEntity task, CancellationToken cancellationToken)
    {
        var taskDb = new TaskDb
        {
            Id = task.Id,
            DisplayName = task.DisplayName,
            Description = task.Description,
            Open = task.Open,
            ProjectId = task.ProjectId
        };

        foreach (var assigneeUserId in task.Assignees)
        {
            var assignee = await this.GetUserAsync(assigneeUserId, cancellationToken);
            taskDb.Assignees.Add(assignee);
        }

        dbContext.Tasks.Add(taskDb);

        return taskDb;
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

    private static TaskEntity MapToEntity(TaskDb task) => new()
    {
        Id = task.Id,
        DisplayName = task.DisplayName,
        Description = task.Description,
        Open = task.Open,
        Assignees = [.. task.Assignees.Select(assignee => assignee.Id)],
        ProjectId = task.ProjectId
    };
}
