namespace DotnetProjectManagement.Project.WebAPI.UnitTests.Modules.ProjectManagement.Domain;

using DotnetProjectManagement.Project.WebAPI.Modules.ProjectManagement.Domain;
using FluentAssertions;
using JetBrains.Annotations;
using Moq;
using Xunit;

[TestSubject(typeof(UserFactory))]
public class UserFactoryTest
{
    private readonly UserFactory userFactory;
    private readonly Mock<IUserRepository> userRepositoryMock = new();

    public UserFactoryTest() => this.userFactory = new UserFactory(this.userRepositoryMock.Object);

    [Fact]
    public void GetUser()
    {
        this.userRepositoryMock
            .Setup(projectRepository => projectRepository.FindUserByUsername("the-username"))
            .Returns(new UserData("the-username", true));

        var user = this.userFactory.GetUser("the-username");

        user.Username.Should().Be("the-username");
        user.IsAdministrator.Should().BeTrue();
    }

    [Fact]
    public void GetUserWithUnknownUsernameThrowsException() =>
        this.userFactory.Invoking(uf => uf.GetUser("the-username"))
            .Should().Throw<UserNotFoundException>()
            .WithMessage("User with username 'the-username' was not found.");
}
