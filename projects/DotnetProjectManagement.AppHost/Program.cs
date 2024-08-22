var builder = DistributedApplication.CreateBuilder(args);

var projectWebApi = builder.AddProject<Projects.DotnetProjectManagement_Project_WebAPI>("project-WebAPI");

builder.AddProject<Projects.DotnetProjectManagement_WebApp>("webApp")
    .WithReference(projectWebApi)
    .WithExternalHttpEndpoints();

builder.Build().Run();
