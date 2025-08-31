namespace DotnetProjectManagement.ProjectManagement.UseCases.Abstractions;

using Domain.Entities;

public interface IActivityRepository
{
    public Task SaveAsync(ProjectCreatedActivity activity, CancellationToken cancellationToken = default);

    public Task SaveAsync(ProjectUpdatedActivity activity, CancellationToken cancellationToken = default);

    public Task SaveAsync(ProjectArchivedActivity activity, CancellationToken cancellationToken = default);

    public Task SaveAsync(ProjectRestoredActivity activity, CancellationToken cancellationToken = default);

    public Task SaveAsync(TaskCreatedActivity activity, CancellationToken cancellationToken = default);

    public Task SaveAsync(TaskUpdatedActivity activity, CancellationToken cancellationToken = default);

    public Task SaveAsync(TaskClosedActivity activity, CancellationToken cancellationToken = default);

    public Task SaveAsync(TaskReopenedActivity activity, CancellationToken cancellationToken = default);
}
