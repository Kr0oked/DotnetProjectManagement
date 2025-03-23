namespace DotnetProjectManagement.AppHost.IntegrationTests;

using System.Net;
using FluentAssertions;
using Xunit;

public class IntegrationTests(AppFixture appFixture) : IClassFixture<AppFixture>
{
    [Fact]
    public async Task GetWebResourceRootReturnsOkStatusCode()
    {
        var response = await appFixture.WebFrontendClient.GetAsync("/");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task ListUsersWithoutAuthenticationReturnsUnauthorizedStatusCode()
    {
        var response = await appFixture.GatewayClient
            .GetAsync("/api/project-management/users?pageNumber=0&pageSize=10");
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
