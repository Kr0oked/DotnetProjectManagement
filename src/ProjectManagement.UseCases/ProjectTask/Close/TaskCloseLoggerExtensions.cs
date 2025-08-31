namespace DotnetProjectManagement.ProjectManagement.UseCases.ProjectTask.Close;

using Microsoft.Extensions.Logging;

internal static partial class TaskCloseLoggerExtensions
{
    [LoggerMessage(
        Level = LogLevel.Information,
        Message = "User {UserId} closed task {TaskId}")]
    public static partial void LogTaskClosed(this ILogger logger, Guid userId, Guid taskId);
}
