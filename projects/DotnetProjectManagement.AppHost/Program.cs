using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var keycloakAdminUsername = builder.AddParameter("keycloakAdminUsername");
var keycloakAdminPassword = builder.AddParameter("keycloakAdminPassword");

var keycloak = builder.AddKeycloak("keycloak", 8080, keycloakAdminUsername, keycloakAdminPassword)
    .WithExternalHttpEndpoints()
    .WithRealmImport("realms", true)
    .WithDataVolume();

var projectWebAPI = builder.AddProject<DotnetProjectManagement_Project_WebAPI>("project-web-api")
    .WithReference(keycloak)
    .WaitFor(keycloak);

var gateway = builder.AddProject<DotnetProjectManagement_Gateway>("gateway")
    .WithExternalHttpEndpoints()
    .WithReference(projectWebAPI)
    .WaitFor(projectWebAPI);

builder.AddProject<DotnetProjectManagement_WebApp>("web-app")
    .WithExternalHttpEndpoints()
    .WaitFor(keycloak)
    .WaitFor(gateway);

builder.Build().Run();
