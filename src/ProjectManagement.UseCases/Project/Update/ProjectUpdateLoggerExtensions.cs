namespace DotnetProjectManagement.ProjectManagement.UseCases.Project.Update;

using Domain.Entities;
using Microsoft.Extensions.Logging;

internal static partial class ProjectUpdateLoggerExtensions
{
    [LoggerMessage(
        Level = LogLevel.Information,
        Message = "User {UserId} updated {Project}")]
    public static partial void LogProjectUpdated(this ILogger logger, Guid userId, Project project);
}
