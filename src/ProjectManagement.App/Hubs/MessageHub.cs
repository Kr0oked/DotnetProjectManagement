namespace DotnetProjectManagement.ProjectManagement.App.Hubs;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

[Authorize]
public class MessageHub : Hub<IMessageClient>;
