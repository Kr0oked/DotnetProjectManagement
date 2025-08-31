using DotnetProjectManagement.ProjectManagement.App.APIs;
using DotnetProjectManagement.ProjectManagement.App.Extensions;
using DotnetProjectManagement.ProjectManagement.Data.Contexts;
using DotnetProjectManagement.ServiceDefaults;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

const string corsDevelopmentPolicy = "CorsDevelopmentPolicy";

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddProblemDetails();

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options => builder.Configuration.Bind("JwtBearer", options));
builder.Services.AddAuthorization();

builder.Services.AddCors(options =>
    options.AddPolicy(corsDevelopmentPolicy,
        policy => policy
            .WithOrigins("http://localhost:5000")
            .AllowAnyHeader()
            .AllowAnyMethod()));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    const string openApi = "OpenAPI";
    options.AddSecurityDefinition(openApi,
        new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.OpenIdConnect,
            OpenIdConnectUrl = new Uri(builder.Configuration.GetValue<string>("JwtBearer:MetadataAddress")!)
        });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = openApi
                }
            },
            []
        }
    });
});

builder.AddNpgsqlDbContext<ProjectManagementDbContext>("project-management-db",
    settings => settings.DisableRetry = true,
    dbContext => dbContext.UseNpgsql(npgsql => npgsql.MigrationsAssembly("ProjectManagement.MigrationService")));

builder.AddApplicationServices();

var app = builder.Build();

app.MapDefaultEndpoints();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.OAuthClientId("swagger");
        options.OAuthScopes("dotnet-roles", "openid", "profile");
    });
}

if (app.Environment.IsDevelopment())
{
    app.UseCors(corsDevelopmentPolicy);
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapProjectApi()
    .WithOpenApi()
    .RequireAuthorization();

app.MapTaskApi()
    .WithOpenApi()
    .RequireAuthorization();

app.MapUserApi()
    .WithOpenApi()
    .RequireAuthorization();

app.Run();
