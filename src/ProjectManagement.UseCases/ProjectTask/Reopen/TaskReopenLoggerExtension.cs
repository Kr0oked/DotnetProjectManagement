namespace DotnetProjectManagement.ProjectManagement.UseCases.ProjectTask.Reopen;

using Microsoft.Extensions.Logging;

internal static partial class TaskReopenLoggerExtension
{
    [LoggerMessage(
        Level = LogLevel.Information,
        Message = "User {UserId} reopened task {TaskId}")]
    public static partial void LogTaskReopened(this ILogger logger, Guid userId, Guid taskId);
}
