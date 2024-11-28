using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var keycloakAdminUsername = builder.AddParameter("keycloakAdminUsername");
var keycloakAdminPassword = builder.AddParameter("keycloakAdminPassword");

var keycloak = builder.AddKeycloak("keycloak", 8080, keycloakAdminUsername, keycloakAdminPassword)
    .WithExternalHttpEndpoints()
    .WithRealmImport("realms", true)
    .WithDataVolume();

var projectManagementApp = builder.AddProject<ProjectManagement_App>("project-management-app")
    .WithReference(keycloak)
    .WaitFor(keycloak);

var gateway = builder.AddProject<Gateway>("gateway")
    .WithExternalHttpEndpoints()
    .WithReference(projectManagementApp)
    .WaitFor(projectManagementApp);

builder.AddProject<WebFrontend>("web-frontend")
    .WithExternalHttpEndpoints()
    .WaitFor(keycloak)
    .WaitFor(gateway);

builder.Build().Run();
