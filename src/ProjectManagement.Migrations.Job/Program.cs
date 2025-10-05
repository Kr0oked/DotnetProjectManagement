using DotnetProjectManagement.ProjectManagement.Data.Contexts;
using DotnetProjectManagement.ProjectManagement.MigrationService;
using DotnetProjectManagement.ServiceDefaults;
using Microsoft.EntityFrameworkCore;

var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();
builder.Services.AddHostedService<Worker>();

builder.Services.AddOpenTelemetry()
    .WithTracing(tracing => tracing.AddSource(Worker.ActivitySourceName));

builder.AddSqlServerDbContext<ProjectManagementDbContext>(
    connectionName: "project-management-db",
    configureDbContextOptions: options =>
        options.UseSqlServer(sqlServer => sqlServer.MigrationsAssembly("ProjectManagement.Migrations")));

var host = builder.Build();
host.Run();
