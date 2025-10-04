namespace DotnetProjectManagement.ProjectManagement.UseCases.Abstractions;

using DTOs;

public interface IMessageBroker
{
    public Task Publish(ProjectActionMessage message, CancellationToken cancellationToken = default);

    public Task Publish(TaskActionMessage message, CancellationToken cancellationToken = default);
}
