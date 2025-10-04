namespace DotnetProjectManagement.ProjectManagement.App.Middlewares;

using Microsoft.Extensions.Logging;

internal static partial class UserInitializationLoggerExtensions
{
    [LoggerMessage(
        Level = LogLevel.Debug,
        Message = "User initialization failed. Missing claim: {missingClaim}")]
    public static partial void LogMissingClaim(this ILogger<UserInitializationMiddleware> logger, string missingClaim);
}
