namespace DotnetProjectManagement.ProjectManagement.UseCases.Project.Create;

using Domain.Entities;
using Microsoft.Extensions.Logging;

internal static partial class ProjectCreateLoggerExtensions
{
    [LoggerMessage(
        Level = LogLevel.Information,
        Message = "User {UserId} created {Project}")]
    public static partial void LogProjectCreated(this ILogger logger, Guid userId, Project project);
}
