namespace DotnetProjectManagement.ProjectManagement.IntegrationTests.Tests;

using System.Net;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;
using static FluentAssertions.FluentActions;

public class UserApiTests(TestWebApplicationFactory<Program> testWebApplicationFactory, ITestOutputHelper output)
    : IntegrationTest(testWebApplicationFactory, output)
{
    [Fact]
    public async Task ListUsers()
    {
        var page1 = await this.UserClient.ListUsersAsync(0, 1);

        page1.Number.Should().Be(0);
        page1.Size.Should().Be(1);
        page1.TotalElements.Should().Be(2);
        page1.TotalPages.Should().Be(2);
        page1.Content.Should().SatisfyRespectively(user =>
            {
                user.Id.Should().Be(DefaultAdminGuid);
                user.FirstName.Should().Be("FirstNameAdmin");
                user.LastName.Should().Be("LastNameAdmin");
            }
        );

        var page2 = await this.UserClient.ListUsersAsync(1, 1);

        page2.Number.Should().Be(1);
        page2.Size.Should().Be(1);
        page2.TotalElements.Should().Be(2);
        page2.TotalPages.Should().Be(2);
        page2.Content.Should().SatisfyRespectively(user =>
            {
                user.Id.Should().Be(DefaultUserGuid);
                user.FirstName.Should().Be("FirstNameUser");
                user.LastName.Should().Be("LastNameUser");
            }
        );
    }

    [Fact]
    public async Task GetUserDetails()
    {
        var user = await this.UserClient.GetUserDetailsAsync(DefaultUserGuid);

        user.Id.Should().Be(DefaultUserGuid);
        user.FirstName.Should().Be("FirstNameUser");
        user.LastName.Should().Be("LastNameUser");
    }

    [Fact]
    public async Task GetUserDetailsWithInvalidUserId() =>
        await Invoking(() => this.UserClient.GetUserDetailsAsync(new Guid("0ca6ec6a-36a2-4de2-8dbe-f3da2206cc88")))
            .Should().ThrowAsync<HttpRequestException>()
            .Where(exception => exception.StatusCode == HttpStatusCode.NotFound);
}
