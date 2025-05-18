namespace DotnetProjectManagement.ProjectManagement.IntegrationTests.Tests;

using System.Net;
using FluentAssertions;
using FS.Keycloak.RestApiClient.Model;
using Xunit;
using Xunit.Abstractions;
using static FluentAssertions.FluentActions;

public class UserApiTests(TestWebApplicationFactory<Program> testWebApplicationFactory, ITestOutputHelper output)
    : IntegrationTest(testWebApplicationFactory, output)
{
    [Fact]
    public async Task ListUsers()
    {
        var keycloakUserA = new UserRepresentation
        {
            Id = new Guid("0ca6ec6a-36a2-4de2-8dbe-f3da2206cc88").ToString(),
            FirstName = "FirstNameA",
            LastName = "LastNameA"
        };
        var keycloakUserB = new UserRepresentation
        {
            Id = new Guid("f7e57387-d18c-4e0e-8d4a-212784c500b3").ToString(),
            FirstName = "FirstNameB",
            LastName = "LastNameB"
        };
        var keycloakUserC = new UserRepresentation
        {
            Id = new Guid("041d3a6d-cb59-4514-bb22-bf2726d7d054").ToString(),
            FirstName = "FirstNameC",
            LastName = "LastNameC"
        };
        this.KeycloakUsers = [keycloakUserA, keycloakUserB, keycloakUserC];

        var page1 = await this.UserClient.ListUsersAsync(0, 2);

        page1.Number.Should().Be(0);
        page1.Size.Should().Be(2);
        page1.TotalElements.Should().Be(3);
        page1.TotalPages.Should().Be(2);
        page1.Content.Should().SatisfyRespectively(
            user =>
            {
                user.Id.ToString().Should().Be(keycloakUserA.Id);
                user.FirstName.Should().Be(keycloakUserA.FirstName);
                user.LastName.Should().Be(keycloakUserA.LastName);
            },
            user =>
            {
                user.Id.ToString().Should().Be(keycloakUserB.Id);
                user.FirstName.Should().Be(keycloakUserB.FirstName);
                user.LastName.Should().Be(keycloakUserB.LastName);
            }
        );

        var page2 = await this.UserClient.ListUsersAsync(1, 2);

        page2.Number.Should().Be(1);
        page2.Size.Should().Be(2);
        page2.TotalElements.Should().Be(3);
        page2.TotalPages.Should().Be(2);
        page2.Content.Should().SatisfyRespectively(user =>
            {
                user.Id.ToString().Should().Be(keycloakUserC.Id);
                user.FirstName.Should().Be(keycloakUserC.FirstName);
                user.LastName.Should().Be(keycloakUserC.LastName);
            }
        );
    }

    [Fact]
    public async Task GetUserDetails()
    {
        var userId = new Guid("0ca6ec6a-36a2-4de2-8dbe-f3da2206cc88");
        var keycloakUser = new UserRepresentation
        {
            Id = userId.ToString(),
            FirstName = "FirstName",
            LastName = "LastName"
        };
        this.KeycloakUsers.Add(keycloakUser);

        var user = await this.UserClient.GetUserDetailsAsync(userId);

        user.Id.Should().Be(userId);
        user.FirstName.Should().Be(keycloakUser.FirstName);
        user.LastName.Should().Be(keycloakUser.LastName);
    }

    [Fact]
    public async Task GetUserDetailsWithInvalidUserId() =>
        await Invoking(() => this.UserClient.GetUserDetailsAsync(new Guid("0ca6ec6a-36a2-4de2-8dbe-f3da2206cc88")))
            .Should().ThrowAsync<HttpRequestException>()
            .Where(exception => exception.StatusCode == HttpStatusCode.NotFound);
}
