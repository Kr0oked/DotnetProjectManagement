var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.DotnetProjectManagement_Project_WebAPI>("project")
    .WithExternalHttpEndpoints();

builder.AddProject<Projects.DotnetProjectManagement_WebApp>("web")
    .WithExternalHttpEndpoints();

builder.Build().Run();
