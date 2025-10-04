namespace DotnetProjectManagement.ProjectManagement.App.Services;

using Extensions;
using UseCases.Abstractions;
using UseCases.DTOs;
using Hubs;
using Microsoft.AspNetCore.SignalR;

public class MessageBroker(IHubContext<MessageHub, IMessageClient> hubContext) : IMessageBroker
{
    public async Task Publish(ProjectActionMessage message, CancellationToken cancellationToken = default) =>
        await hubContext.Clients.Users(GetUserIds(message.Project))
            .ReceiveProjectAction(message.ToWeb());

    public async Task Publish(TaskActionMessage message, CancellationToken cancellationToken = default) =>
        await hubContext.Clients.Users(GetUserIds(message.Project))
            .ReceiveTaskAction(message.ToWeb());

    private static IReadOnlyList<string> GetUserIds(ProjectDto project) =>
        [.. project.Members.Keys.Select(userId => userId.ToString())];
}
