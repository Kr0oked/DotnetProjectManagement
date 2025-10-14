namespace DotnetProjectManagement.ProjectManagement.App.Extensions;

using Jobs;
using Services;
using UseCases.Abstractions;
using UseCases.Project.Archive;
using UseCases.Project.Create;
using UseCases.Project.GetDetails;
using UseCases.Project.GetHistory;
using UseCases.Project.List;
using UseCases.Project.Restore;
using UseCases.Project.Update;
using UseCases.ProjectTask.Close;
using UseCases.ProjectTask.Create;
using UseCases.ProjectTask.GetDetails;
using UseCases.ProjectTask.GetHistory;
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

        services.AddHttpClient<ExternalResourceService>(client =>
            client.BaseAddress = new Uri("http+http://external-resource"));

        services.AddTransient<ImportUsersJob>();

        services.AddScoped<IMessageBroker, MessageBroker>();

        services.AddScoped<IProjectRepository, ProjectRepository>();
        services.AddScoped<ITaskRepository, TaskRepository>();
        services.AddScoped<ITransactionManager, TransactionManager>();
        services.AddScoped<IUserRepository, UserRepository>();

        services.AddScoped<ProjectArchiveUseCase>();
        services.AddScoped<ProjectCreateUseCase>();
        services.AddScoped<ProjectGetDetailsUseCase>();
        services.AddScoped<ProjectGetHistoryUseCase>();
        services.AddScoped<ProjectListUseCase>();
        services.AddScoped<ProjectRestoreUseCase>();
        services.AddScoped<ProjectUpdateUseCase>();

        services.AddScoped<TaskCloseUseCase>();
        services.AddScoped<TaskCreateUseCase>();
        services.AddScoped<TaskGetDetailsUseCase>();
        services.AddScoped<TaskGetHistoryUseCase>();
        services.AddScoped<TaskListForProjectUseCase>();
        services.AddScoped<TaskReopenUseCase>();
        services.AddScoped<TaskUpdateUseCase>();

        services.AddScoped<UserGetDetailsUseCase>();
        services.AddScoped<UserInitializationUseCase>();
        services.AddScoped<UserListUseCase>();
    }
}
