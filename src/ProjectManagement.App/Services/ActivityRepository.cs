namespace DotnetProjectManagement.ProjectManagement.App.Services;

using Data.Contexts;
using Data.Models;
using UseCases.Abstractions;
using UseCases.Exceptions;
using ProjectArchivedActivityEntity = Domain.Entities.ProjectArchivedActivity;
using ProjectCreatedActivityEntity = Domain.Entities.ProjectCreatedActivity;
using ProjectRestoredActivityEntity = Domain.Entities.ProjectRestoredActivity;
using ProjectUpdatedActivityEntity = Domain.Entities.ProjectUpdatedActivity;
using TaskCreatedActivityEntity = Domain.Entities.TaskCreatedActivity;
using TaskUpdatedActivityEntity = Domain.Entities.TaskUpdatedActivity;
using TaskClosedActivityEntity = Domain.Entities.TaskClosedActivity;
using TaskReopenedActivityEntity = Domain.Entities.TaskReopenedActivity;
using ProjectArchivedActivityDb = Data.Models.ProjectArchivedActivity;
using ProjectCreatedActivityDb = Data.Models.ProjectCreatedActivity;
using ProjectRestoredActivityDb = Data.Models.ProjectRestoredActivity;
using ProjectUpdatedActivityDb = Data.Models.ProjectUpdatedActivity;
using TaskCreatedActivityDb = Data.Models.TaskCreatedActivity;
using TaskUpdatedActivityDb = Data.Models.TaskUpdatedActivity;
using TaskClosedActivityDb = Data.Models.TaskClosedActivity;
using TaskReopenedActivityDb = Data.Models.TaskReopenedActivity;
using User = Data.Models.User;

public class ActivityRepository(ProjectManagementDbContext dbContext) : IActivityRepository
{
    public async Task SaveAsync(ProjectCreatedActivityEntity activity, CancellationToken cancellationToken = default)
    {
        var activityDb = new ProjectCreatedActivityDb
        {
            UserId = activity.UserId,
            User = await this.GetUserAsync(activity.UserId, cancellationToken),
            Timestamp = activity.Timestamp,
            ProjectId = activity.ProjectId,
            DisplayName = activity.DisplayName
        };

        foreach (var (userId, role) in activity.Members)
        {
            activityDb.Members.Add(new ProjectCreatedActivityMember
            {
                ProjectCreatedActivityId = activityDb.Id,
                UserId = userId,
                User = await this.GetUserAsync(activity.UserId, cancellationToken),
                Role = role
            });
        }

        dbContext.ProjectCreatedActivities.Add(activityDb);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task SaveAsync(ProjectUpdatedActivityEntity activity, CancellationToken cancellationToken = default)
    {
        var activityDb = new ProjectUpdatedActivityDb
        {
            UserId = activity.UserId,
            User = await this.GetUserAsync(activity.UserId, cancellationToken),
            Timestamp = activity.Timestamp,
            ProjectId = activity.ProjectId,
            OldDisplayName = activity.OldDisplayName,
            NewDisplayName = activity.NewDisplayName
        };

        foreach (var (userId, role) in activity.OldMembers)
        {
            activityDb.Members.Add(new ProjectUpdatedActivityMember
            {
                UserId = userId,
                User = await this.GetUserAsync(activity.UserId, cancellationToken),
                ProjectUpdatedActivityId = activityDb.Id,
                New = false,
                Role = role
            });
        }

        foreach (var (userId, role) in activity.NewMembers)
        {
            activityDb.Members.Add(new ProjectUpdatedActivityMember
            {
                UserId = userId,
                User = await this.GetUserAsync(activity.UserId, cancellationToken),
                ProjectUpdatedActivityId = activityDb.Id,
                New = true,
                Role = role
            });
        }

        dbContext.ProjectUpdatedActivities.Add(activityDb);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task SaveAsync(ProjectArchivedActivityEntity activity, CancellationToken cancellationToken = default)
    {
        var activityDb = new ProjectArchivedActivityDb
        {
            UserId = activity.UserId,
            User = await this.GetUserAsync(activity.UserId, cancellationToken),
            Timestamp = activity.Timestamp,
            ProjectId = activity.ProjectId
        };

        dbContext.ProjectArchivedActivities.Add(activityDb);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task SaveAsync(ProjectRestoredActivityEntity activity, CancellationToken cancellationToken = default)
    {
        var activityDb = new ProjectRestoredActivityDb
        {
            UserId = activity.UserId,
            User = await this.GetUserAsync(activity.UserId, cancellationToken),
            Timestamp = activity.Timestamp,
            ProjectId = activity.ProjectId
        };

        dbContext.ProjectRestoredActivities.Add(activityDb);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task SaveAsync(TaskCreatedActivityEntity activity, CancellationToken cancellationToken = default)
    {
        var activityDb = new TaskCreatedActivityDb
        {
            UserId = activity.UserId,
            User = await this.GetUserAsync(activity.UserId, cancellationToken),
            Timestamp = activity.Timestamp,
            TaskId = activity.TaskId,
            DisplayName = activity.DisplayName,
            Description = activity.Description
        };

        dbContext.TaskCreatedActivities.Add(activityDb);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task SaveAsync(TaskUpdatedActivityEntity activity, CancellationToken cancellationToken = default)
    {
        var activityDb = new TaskUpdatedActivityDb
        {
            UserId = activity.UserId,
            User = await this.GetUserAsync(activity.UserId, cancellationToken),
            Timestamp = activity.Timestamp,
            TaskId = activity.TaskId,
            NewDisplayName = activity.NewDisplayName,
            OldDisplayName = activity.OldDisplayName,
            NewDescription = activity.NewDescription,
            OldDescription = activity.OldDescription
        };

        foreach (var newAssigneeUserId in activity.NewAssignees)
        {
            var newAssignee = await this.GetUserAsync(newAssigneeUserId, cancellationToken);
            activityDb.NewAssignees.Add(newAssignee);
        }

        foreach (var oldAssigneeUserId in activity.OldAssignees)
        {
            var oldAssignee = await this.GetUserAsync(oldAssigneeUserId, cancellationToken);
            activityDb.OldAssignees.Add(oldAssignee);
        }

        dbContext.TaskUpdatedActivities.Add(activityDb);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task SaveAsync(TaskClosedActivityEntity activity, CancellationToken cancellationToken = default)
    {
        var activityDb = new TaskClosedActivityDb
        {
            UserId = activity.UserId,
            User = await this.GetUserAsync(activity.UserId, cancellationToken),
            Timestamp = activity.Timestamp,
            TaskId = activity.TaskId
        };

        dbContext.TaskClosedActivities.Add(activityDb);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task SaveAsync(TaskReopenedActivityEntity activity, CancellationToken cancellationToken = default)
    {
        var activityDb = new TaskReopenedActivityDb
        {
            UserId = activity.UserId,
            User = await this.GetUserAsync(activity.UserId, cancellationToken),
            Timestamp = activity.Timestamp,
            TaskId = activity.TaskId
        };

        dbContext.TaskReopenedActivities.Add(activityDb);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task<User> GetUserAsync(Guid userId, CancellationToken cancellationToken) =>
        await dbContext.Users.FindAsync([userId], cancellationToken) ?? throw new UserNotFoundException(userId);
}
