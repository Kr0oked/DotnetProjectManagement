namespace DotnetProjectManagement.ProjectManagement.UseCases.ProjectTask.Create;

using Domain.Entities;
using Microsoft.Extensions.Logging;

internal static partial class TaskCreateLoggerExtensions
{
    [LoggerMessage(
        Level = LogLevel.Information,
        Message = "User {UserId} created {Task}")]
    public static partial void LogTaskCreated(this ILogger logger, Guid userId, ProjectTask task);
}
