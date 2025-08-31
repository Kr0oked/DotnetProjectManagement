namespace DotnetProjectManagement.ProjectManagement.UseCases.UnitTests.Extensions;

using Domain.Entities;
using FluentAssertions;
using UseCases.Extensions;
using Xunit;

public class UserExtensionsTests
{
    [Fact]
    public void ToDto()
    {
        var userId = new Guid("5cc2a903-7672-4f78-b360-b573b6469ee3");
        var user = new User
        {
            Id = userId,
            FirstName = "FirstName",
            LastName = "LastName"
        };

        var dto = user.ToDto();

        dto.Id.Should().Be(userId);
        dto.FirstName.Should().Be("FirstName");
        dto.LastName.Should().Be("LastName");
    }
}
