using DotnetProjectManagement.ProjectManagement.App.APIs;
using DotnetProjectManagement.ProjectManagement.App.Extensions;
using DotnetProjectManagement.ProjectManagement.App.Hubs;
using DotnetProjectManagement.ProjectManagement.App.Middlewares;
using DotnetProjectManagement.ProjectManagement.Data.Contexts;
using DotnetProjectManagement.ServiceDefaults;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;

const string corsDevelopmentPolicy = "CorsDevelopmentPolicy";

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddSignalR()
    .AddStackExchangeRedis(builder.Configuration.GetConnectionString("distributed-cache")!);

builder.Services.AddProblemDetails();

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var accessToken = context.Request.Query["access_token"];

                var path = context.HttpContext.Request.Path;
                if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs/messages"))
                {
                    context.Token = accessToken;
                }

                return Task.CompletedTask;
            }
        };
        builder.Configuration.Bind("JwtBearer", options);
    });
builder.Services.AddAuthorization();

builder.Services.AddCors(options =>
    options.AddPolicy(corsDevelopmentPolicy,
        policy => policy
            .WithOrigins("http://localhost:5000")
            .AllowAnyHeader()
            .AllowAnyMethod()));

builder.Services.AddOpenApi(options => options.AddDocumentTransformer((document, _, _) =>
{
    document.Components = new OpenApiComponents
    {
        SecuritySchemes = new Dictionary<string, IOpenApiSecurityScheme>
        {
            {
                "OpenIdConnect", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.OpenIdConnect,
                    OpenIdConnectUrl = new Uri(builder.Configuration.GetValue<string>("JwtBearer:MetadataAddress")!)
                }
            }
        }
    };

    foreach (var operation in document.Paths.Values.SelectMany(path => path.Operations ?? []))
    {
        operation.Value.Security =
        [
            new OpenApiSecurityRequirement { [new OpenApiSecuritySchemeReference("OpenIdConnect", document)] = [] }
        ];
    }

    return Task.CompletedTask;
}));

builder.AddSqlServerDbContext<ProjectManagementDbContext>("project-management-db",
    settings => settings.DisableRetry = true,
    dbContext => dbContext
        .UseSqlServer(sqlServer => sqlServer.MigrationsAssembly("ProjectManagement.Migrations")));

builder.AddApplicationServices();

var app = builder.Build();

app.UseHttpsRedirection();

if (app.Environment.IsDevelopment())
{
    app.UseCors(corsDevelopmentPolicy);
}

app.UseAuthentication();
app.UseAuthorization();

app.UseUserInitialization();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "v1");
        options.OAuthClientId("swagger");
        options.OAuthScopes("dotnet-roles", "openid", "profile");
    });
}

app.MapDefaultEndpoints();

app.MapHub<MessageHub>("/hubs/messages");

app.MapProjectApi()
    .RequireAuthorization();

app.MapTaskApi()
    .RequireAuthorization();

app.MapUserApi()
    .RequireAuthorization();

app.Run();
