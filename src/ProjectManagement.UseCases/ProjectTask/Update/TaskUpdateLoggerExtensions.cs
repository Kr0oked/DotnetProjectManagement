namespace DotnetProjectManagement.ProjectManagement.UseCases.ProjectTask.Update;

using Domain.Entities;
using Microsoft.Extensions.Logging;

internal static partial class TaskUpdateLoggerExtensions
{
    [LoggerMessage(
        Level = LogLevel.Information,
        Message = "User {UserId} updated {Task}")]
    public static partial void LogTaskUpdated(this ILogger logger, Guid userId, ProjectTask task);
}
