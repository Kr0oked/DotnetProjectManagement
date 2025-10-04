namespace DotnetProjectManagement.ProjectManagement.IntegrationTests;

using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;
using Data.Contexts;
using Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using UseCases.Abstractions;
using Web.Clients;
using Xunit;
using Xunit.Abstractions;

[Collection("IntegrationTests")]
[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
public abstract class IntegrationTest : IClassFixture<TestWebApplicationFactory<Program>>
{
    protected const string DefaultUserId = "6cf3ee68-3e83-4705-acca-7a1301d1c010";
    protected const string DefaultAdminId = "562f0563-9a86-40fc-8411-11bc1f2684c7";
    protected static readonly Guid DefaultUserGuid = new(DefaultUserId);
    protected static readonly Guid DefaultAdminGuid = new(DefaultAdminId);

    private readonly TestWebApplicationFactory<Program> webApplicationFactory;

    protected Mock<IMessageBroker> MessageBrokerMock { get; }
    protected UserClient UserClient { get; }
    protected ProjectClient ProjectClient { get; }
    protected TaskClient TaskClient { get; }

    protected IntegrationTest(TestWebApplicationFactory<Program> webApplicationFactory, ITestOutputHelper output)
    {
        this.webApplicationFactory = webApplicationFactory;
        this.webApplicationFactory.OutputHelper = output;

        this.MessageBrokerMock = this.webApplicationFactory.MessageBrokerMock;
        this.UserClient = new UserClient(this.webApplicationFactory.CreateClient());
        this.ProjectClient = new ProjectClient(this.webApplicationFactory.CreateClient());
        this.TaskClient = new TaskClient(this.webApplicationFactory.CreateClient());

        this.MigrateDatabase();
        this.CleanupDatabase();
        this.SetupUsersInDatabase();

        this.ResetMocks();
        this.ActAsUser();
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

    private void SetupUsersInDatabase()
    {
        using var scope = this.webApplicationFactory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ProjectManagementDbContext>();
        dbContext.Users.Add(new User
        {
            Id = DefaultAdminGuid,
            FirstName = "FirstNameAdmin",
            LastName = "LastNameAdmin"
        });
        dbContext.Users.Add(new User
        {
            Id = DefaultUserGuid,
            FirstName = "FirstNameUser",
            LastName = "LastNameUser"
        });
        dbContext.SaveChanges();
    }

    protected void ResetMocks() =>
        this.webApplicationFactory.MessageBrokerMock.Reset();

    protected void ActAsUser(string userId = DefaultUserId) =>
        this.ActAsUser(new Guid(userId));

    protected void ActAsUser(Guid userId)
    {
        this.webApplicationFactory.ClaimsProvider.Claims.Clear();
        this.webApplicationFactory.ClaimsProvider.Claims.AddRange([
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim(ClaimTypes.GivenName, "FirstNameUser"),
            new Claim(ClaimTypes.Surname, "LastNameUser"),
        ]);
    }

    protected void ActAsAdmin(string userId = DefaultAdminId) =>
        this.ActAsAdmin(new Guid(userId));

    protected void ActAsAdmin(Guid userId)
    {
        this.webApplicationFactory.ClaimsProvider.Claims.Clear();
        this.webApplicationFactory.ClaimsProvider.Claims.AddRange([
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim(ClaimTypes.GivenName, "FirstNameAdmin"),
            new Claim(ClaimTypes.Surname, "LastNameAdmin"),
            new Claim(ClaimTypes.Role, "app_admin")
        ]);
    }
}
