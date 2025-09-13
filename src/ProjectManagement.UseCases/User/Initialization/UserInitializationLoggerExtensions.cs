namespace DotnetProjectManagement.ProjectManagement.UseCases.User.Initialization;

using Domain.Entities;
using Microsoft.Extensions.Logging;

internal static partial class UserInitializationLoggerExtensions
{
    [LoggerMessage(
        Level = LogLevel.Trace,
        Message = "Found existing user for {userId}")]
    public static partial void LogExistingUserFound(this ILogger<UserInitializationUseCase> logger, Guid userId);

    [LoggerMessage(
        Level = LogLevel.Information,
        Message = "Initialized {user}")]
    public static partial void LogInitializedUser(this ILogger<UserInitializationUseCase> logger, User user);
}
