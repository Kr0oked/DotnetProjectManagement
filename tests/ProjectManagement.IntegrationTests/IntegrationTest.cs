namespace DotnetProjectManagement.ProjectManagement.IntegrationTests;

using System.Security.Claims;
using Data.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Web.Clients;
using Xunit;
using Xunit.Abstractions;
using UserRepresentation = FS.Keycloak.RestApiClient.Model.UserRepresentation;

[Collection("IntegrationTests")]
public abstract class IntegrationTest : IClassFixture<TestWebApplicationFactory<Program>>
{
    protected const string DefaultUserId = "6cf3ee68-3e83-4705-acca-7a1301d1c010";
    protected const string DefaultAdminId = "562f0563-9a86-40fc-8411-11bc1f2684c7";
    protected static readonly Guid DefaultUserGuid = new(DefaultUserId);
    protected static readonly Guid DefaultAdminGuid = new(DefaultAdminId);

    private readonly TestWebApplicationFactory<Program> webApplicationFactory;

    protected IntegrationTest(TestWebApplicationFactory<Program> webApplicationFactory, ITestOutputHelper output)
    {
        this.webApplicationFactory = webApplicationFactory;
        this.webApplicationFactory.OutputHelper = output;

        this.UserClient = new UserClient(this.webApplicationFactory.CreateClient());
        this.ProjectClient = new ProjectClient(this.webApplicationFactory.CreateClient());

        this.MigrateDatabase();
        this.CleanupDatabase();
        this.KeycloakUsers =
        [
            new UserRepresentation
            {
                Id = DefaultAdminId,
                FirstName = "FirstNameAdmin",
                LastName = "LastNameAdmin"
            },
            new UserRepresentation
            {
                Id = DefaultUserId,
                FirstName = "FirstNameUser",
                LastName = "LastNameUser"
            }
        ];
        this.ActAsUser();
    }

    protected UserClient UserClient { get; }
    protected ProjectClient ProjectClient { get; }

    protected List<UserRepresentation> KeycloakUsers
    {
        get => this.webApplicationFactory.UsersApiFake.Users;
        set => this.webApplicationFactory.UsersApiFake.Users = value;
    }

    private void MigrateDatabase()
    {
        using var scope = this.webApplicationFactory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ProjectManagementDbContext>();
        dbContext.Database.Migrate();
    }

    private void CleanupDatabase()
    {
        using var scope = this.webApplicationFactory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ProjectManagementDbContext>();
        dbContext.Projects.ExecuteDelete();
        dbContext.Users.ExecuteDelete();
    }

    protected void ActAsUser(string userId = DefaultUserId) =>
        this.ActAsUser(new Guid(userId));

    protected void ActAsUser(Guid userId)
    {
        this.webApplicationFactory.ClaimsProvider.Claims.Clear();
        this.webApplicationFactory.ClaimsProvider.Claims.AddRange([
            new Claim(ClaimTypes.Name, userId.ToString())
        ]);
    }

    protected void ActAsAdmin(string userId = DefaultAdminId) =>
        this.ActAsAdmin(new Guid(userId));

    protected void ActAsAdmin(Guid userId)
    {
        this.webApplicationFactory.ClaimsProvider.Claims.Clear();
        this.webApplicationFactory.ClaimsProvider.Claims.AddRange([
            new Claim(ClaimTypes.Name, userId.ToString()),
            new Claim(ClaimTypes.Role, "app_admin")
        ]);
    }
}
