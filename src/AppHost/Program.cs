using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var keycloakAdminUsername = builder.AddParameter("keycloakAdminUsername", true);
var keycloakAdminPassword = builder.AddParameter("keycloakAdminPassword", true);
var keycloak = builder.AddKeycloak("keycloak", 8080, keycloakAdminUsername, keycloakAdminPassword)
    .WithExternalHttpEndpoints()
    .WithRealmImport("realms")
    .WithDataVolume();

var sqlPassword = builder.AddParameter("sqlPassword", true);
var sql = builder.AddSqlServer("sql", sqlPassword)
    .WithDataVolume();

var projectManagementDatabase = sql.AddDatabase("project-management-db");

var projectManagementMigrationService = builder.AddProject<ProjectManagement_MigrationService>("migration-service")
    .WithReference(projectManagementDatabase)
    .WaitFor(projectManagementDatabase);

var projectManagementApp = builder.AddProject<ProjectManagement_App>("project-management-app")
    .WithReference(keycloak)
    .WithReference(projectManagementDatabase)
    .WaitFor(keycloak)
    .WaitFor(projectManagementDatabase)
    .WaitForCompletion(projectManagementMigrationService);

var gateway = builder.AddProject<Gateway>("gateway")
    .WithExternalHttpEndpoints()
    .WithReference(projectManagementApp)
    .WaitFor(projectManagementApp);

builder.AddProject<WebFrontend>("web-frontend")
    .WithExternalHttpEndpoints()
    .WaitFor(keycloak)
    .WaitFor(gateway);

builder.Build().Run();
