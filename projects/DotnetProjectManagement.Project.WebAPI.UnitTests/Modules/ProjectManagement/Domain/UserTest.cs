namespace DotnetProjectManagement.Project.WebAPI.UnitTests.Modules.ProjectManagement.Domain;

using DotnetProjectManagement.Project.WebAPI.Modules.ProjectManagement.Domain;
using FluentAssertions;
using JetBrains.Annotations;
using Xunit;

[TestSubject(typeof(User))]
public class UserTest
{
    [Fact]
    public void UserProperties()
    {
        var userData = new UserData("the-username", true);

        var user = new User(userData);

        user.Username.Should().Be("the-username");
        user.IsAdministrator.Should().BeTrue();
    }
}
