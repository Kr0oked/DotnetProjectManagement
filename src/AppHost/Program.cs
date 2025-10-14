using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var externalResource = builder.AddDockerfile("external-resource", "external-resource")
    .WithHttpEndpoint(4010, 4010, "http")
    .WithExternalHttpEndpoints();
var externalResourceEndpoint = externalResource.GetEndpoint("http");

var keycloakAdminUsername = builder.AddParameter("keycloakAdminUsername", true);
var keycloakAdminPassword = builder.AddParameter("keycloakAdminPassword", true);
var keycloak = builder.AddKeycloak("keycloak", 8080, keycloakAdminUsername, keycloakAdminPassword)
    .WithExternalHttpEndpoints()
    .WithRealmImport("realms")
    .WithDataVolume();

var distributedCache = builder.AddValkey("distributed-cache");

var sqlPassword = builder.AddParameter("sqlPassword", true);
var sql = builder.AddSqlServer("sql", sqlPassword)
    .WithDataVolume();

var projectManagementDatabase = sql.AddDatabase("project-management-db");

var projectManagementMigrationsJob = builder.AddProject<ProjectManagement_Migrations_Job>("migrations-job")
    .WithReference(projectManagementDatabase)
    .WaitFor(projectManagementDatabase);

var projectManagementApp = builder.AddProject<ProjectManagement_App>("project-management-app")
    .WithReference(externalResourceEndpoint)
    .WithReference(keycloak)
    .WithReference(distributedCache)
    .WithReference(projectManagementDatabase)
    .WaitFor(externalResource)
    .WaitFor(keycloak)
    .WaitFor(distributedCache)
    .WaitFor(projectManagementDatabase)
    .WaitForCompletion(projectManagementMigrationsJob);

var gateway = builder.AddProject<Gateway>("gateway")
    .WithExternalHttpEndpoints()
    .WithReference(projectManagementApp)
    .WaitFor(projectManagementApp);

builder.AddProject<WebFrontend>("web-frontend")
    .WithExternalHttpEndpoints()
    .WaitFor(keycloak)
    .WaitFor(gateway);

builder.Build().Run();
