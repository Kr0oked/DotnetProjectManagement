namespace DotnetProjectManagement.ProjectManagement.App.Middlewares;

using System.Diagnostics.CodeAnalysis;
using Extensions;
using UseCases.User.Initialization;

public class UserInitializationMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext httpContext, UserInitializationUseCase userInitializationUseCase)
    {
        var actor = httpContext.User.ToActor();
        await userInitializationUseCase.InitializeUserAsync(actor);
        await next(httpContext);
    }
}

public static class UserInitializationMiddlewareExtensions
{
    [SuppressMessage("ReSharper", "UnusedMethodReturnValue.Global")]
    public static IApplicationBuilder UseUserInitialization(this IApplicationBuilder builder) =>
        builder.UseMiddleware<UserInitializationMiddleware>();
}
