namespace DotnetProjectManagement.ProjectManagement.App.Extensions;

using Services;
using UseCases.Abstractions;
using UseCases.Project.Archive;
using UseCases.Project.Create;
using UseCases.Project.GetDetails;
using UseCases.Project.List;
using UseCases.Project.Restore;
using UseCases.Project.Update;
using UseCases.ProjectTask.Close;
using UseCases.ProjectTask.Create;
using UseCases.ProjectTask.GetDetails;
using UseCases.ProjectTask.ListForProject;
using UseCases.ProjectTask.Reopen;
using UseCases.ProjectTask.Update;
using UseCases.User.GetDetails;
using UseCases.User.Initialization;
using UseCases.User.List;

internal static class ProgramExtensions
{
    public static void AddApplicationServices(this IHostApplicationBuilder builder)
    {
        var services = builder.Services;

        services.AddScoped<IActivityRepository, ActivityRepository>();
        services.AddScoped<IProjectRepository, ProjectRepository>();
        services.AddScoped<ITaskRepository, TaskRepository>();
        services.AddScoped<ITransactionManager, TransactionManager>();
        services.AddScoped<IUserRepository, UserRepository>();

        services.AddScoped<ProjectArchiveUseCase, ProjectArchiveUseCase>();
        services.AddScoped<ProjectCreateUseCase, ProjectCreateUseCase>();
        services.AddScoped<ProjectGetDetailsUseCase, ProjectGetDetailsUseCase>();
        services.AddScoped<ProjectListUseCase, ProjectListUseCase>();
        services.AddScoped<ProjectRestoreUseCase, ProjectRestoreUseCase>();
        services.AddScoped<ProjectUpdateUseCase, ProjectUpdateUseCase>();

        services.AddScoped<TaskCloseUseCase, TaskCloseUseCase>();
        services.AddScoped<TaskCreateUseCase, TaskCreateUseCase>();
        services.AddScoped<TaskGetDetailsUseCase, TaskGetDetailsUseCase>();
        services.AddScoped<TaskListForProjectUseCase, TaskListForProjectUseCase>();
        services.AddScoped<TaskReopenUseCase, TaskReopenUseCase>();
        services.AddScoped<TaskUpdateUseCase, TaskUpdateUseCase>();

        services.AddScoped<UserGetDetailsUseCase, UserGetDetailsUseCase>();
        services.AddScoped<UserInitializationUseCase, UserInitializationUseCase>();
        services.AddScoped<UserListUseCase, UserListUseCase>();
    }
}
