using DotnetProjectManagement.ProjectManagement.MigrationService;
using DotnetProjectManagement.ProjectManagement.Data.Contexts;
using DotnetProjectManagement.ServiceDefaults;
using Microsoft.EntityFrameworkCore;

var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();
builder.Services.AddHostedService<Worker>();

builder.Services.AddOpenTelemetry()
    .WithTracing(tracing => tracing.AddSource(Worker.ActivitySourceName));

builder.AddNpgsqlDbContext<ProjectManagementDbContext>("project-management-db",
    configureDbContextOptions: options =>
        options.UseNpgsql(npgsql => npgsql.MigrationsAssembly(typeof(Program).Assembly.FullName)));

var host = builder.Build();
host.Run();
