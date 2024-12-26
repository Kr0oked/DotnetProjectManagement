namespace DotnetProjectManagement.ProjectManagement.App.Services;

using Data.Contexts;
using Data.Models;
using UseCases.Abstractions;
using ProjectArchivedActivityEntity = Domain.Entities.ProjectArchivedActivity;
using ProjectCreatedActivityEntity = Domain.Entities.ProjectCreatedActivity;
using ProjectRestoredActivityEntity = Domain.Entities.ProjectRestoredActivity;
using ProjectUpdatedActivityEntity = Domain.Entities.ProjectUpdatedActivity;
using ProjectArchivedActivityDb = Data.Models.ProjectArchivedActivity;
using ProjectCreatedActivityDb = Data.Models.ProjectCreatedActivity;
using ProjectRestoredActivityDb = Data.Models.ProjectRestoredActivity;
using ProjectUpdatedActivityDb = Data.Models.ProjectUpdatedActivity;

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
            Timestamp = activity.Timestamp,
            ProjectId = activity.ProjectId,
            OldDisplayName = activity.OldDisplayName,
            NewDisplayName = activity.NewDisplayName
        };

        foreach (var (userId, role) in activity.OldMembers)
        {
            activityDb.Members.Add(new ProjectUpdatedActivityMember
            {
                ProjectUpdatedActivityId = activityDb.Id,
                UserId = userId,
                User = await this.GetUserAsync(activity.UserId, cancellationToken),
                New = false,
                Role = role
            });
        }

        foreach (var (userId, role) in activity.NewMembers)
        {
            activityDb.Members.Add(new ProjectUpdatedActivityMember
            {
                ProjectUpdatedActivityId = activityDb.Id,
                UserId = userId,
                User = await this.GetUserAsync(activity.UserId, cancellationToken),
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
}
