var builder = DistributedApplication.CreateBuilder(args);

var keycloakAdminUsername = builder.AddParameter("keycloakAdminUsername");
var keycloakAdminPassword = builder.AddParameter("keycloakAdminPassword");

var keycloak = builder.AddKeycloak("keycloak", 8080, keycloakAdminUsername, keycloakAdminPassword)
    .WithExternalHttpEndpoints()
    .WithRealmImport("realms", true)
    .WithDataVolume();

var projectWebAPI = builder.AddProject<Projects.DotnetProjectManagement_Project_WebAPI>("project-web-api")
    .WithReference(keycloak);

builder.AddProject<Projects.DotnetProjectManagement_Gateway>("gateway")
    .WithExternalHttpEndpoints()
    .WithReference(projectWebAPI);

builder.AddProject<Projects.DotnetProjectManagement_WebApp>("web-app")
    .WithExternalHttpEndpoints();


builder.Build().Run();
