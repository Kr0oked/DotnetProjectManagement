namespace DotnetProjectManagement.ProjectManagement.UseCases.Project.Restore;

using Microsoft.Extensions.Logging;

internal static partial class ProjectRestoreLoggerExtensions
{
    [LoggerMessage(
        Level = LogLevel.Information,
        Message = "User {UserId} restored project {ProjectId}")]
    public static partial void LogProjectRestored(this ILogger logger, Guid userId, Guid projectId);
}
