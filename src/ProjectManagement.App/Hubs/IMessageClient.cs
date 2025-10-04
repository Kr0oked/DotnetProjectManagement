namespace DotnetProjectManagement.ProjectManagement.App.Hubs;

using Web.Models;

public interface IMessageClient
{
    public Task ReceiveProjectAction(ProjectActionMessageRepresentation message);

    public Task ReceiveTaskAction(TaskActionMessageRepresentation message);
}
