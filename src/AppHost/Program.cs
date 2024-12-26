using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var keycloakAdminUsername = builder.AddParameter("keycloakAdminUsername", secret: true);
var keycloakAdminPassword = builder.AddParameter("keycloakAdminPassword", secret: true);
var keycloak = builder.AddKeycloak("keycloak", 8080, keycloakAdminUsername, keycloakAdminPassword)
    .WithExternalHttpEndpoints()
    .WithRealmImport("realms", true)
    .WithDataVolume();

var postgresUsername = builder.AddParameter("postgresUsername", secret: true);
var postgresPassword = builder.AddParameter("postgresPassword", secret: true);
var postgres = builder.AddPostgres("postgres", postgresUsername, postgresPassword)
    .WithDataVolume()
    .WithPgAdmin();

var projectManagementDatabase = postgres
    .AddDatabase("project-management-db");

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
