namespace DotnetProjectManagement.ProjectManagement.UseCases.UnitTests.User.List;

using Abstractions;
using Domain.Entities;
using FluentAssertions;
using Moq;
using UseCases.DTOs;
using UseCases.User.List;
using Xunit;

public class UserListUseCaseTests
{
    private readonly UserListUseCase userListUseCase;
    private readonly Mock<IUserRepository> userRepositoryMock = new();

    public UserListUseCaseTests() => this.userListUseCase = new UserListUseCase(this.userRepositoryMock.Object);

    [Fact]
    public async Task ListUsersAsync()
    {
        var pageRequest = new PageRequest(0, 10);
        var cancellationToken = CancellationToken.None;
        var userId = new Guid("00aaba25-ffad-4564-bffc-57a3b85fc171");
        var user = new User
        {
            Id = userId,
            FirstName = "FirstName",
            LastName = "LastName"
        };

        this.userRepositoryMock
            .Setup(userRepository => userRepository.FindAllAsync(pageRequest, cancellationToken))
            .ReturnsAsync(new Page<User>([user], pageRequest, 1));

        var page = await this.userListUseCase.ListUsersAsync(pageRequest, cancellationToken);

        page.Size.Should().Be(10);
        page.TotalElements.Should().Be(1);
        page.TotalPages.Should().Be(1);
        page.Number.Should().Be(0);
        page.Content.Should().SatisfyRespectively(userDto =>
        {
            userDto.Id.Should().Be(userId);
            userDto.FirstName.Should().Be("FirstName");
            userDto.LastName.Should().Be("LastName");
        });
    }
}
