namespace DotnetProjectManagement.ProjectManagement.UseCases.Project.Archive;

using Microsoft.Extensions.Logging;

internal static partial class ProjectArchiveLoggerExtensions
{
    [LoggerMessage(
        Level = LogLevel.Information,
        Message = "User {UserId} archived project {ProjectId}")]
    public static partial void LogProjectArchived(this ILogger logger, Guid userId, Guid projectId);
}
