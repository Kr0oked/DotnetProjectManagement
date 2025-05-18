namespace DotnetProjectManagement.ProjectManagement.UseCases.UnitTests.User.GetDetails;

using Abstractions;
using Domain.Entities;
using Exceptions;
using FluentAssertions;
using Moq;
using UseCases.User.GetDetails;
using Xunit;
using static FluentAssertions.FluentActions;

public class UserGetDetailsUseCaseTests
{
    private readonly UserGetDetailsUseCase userGetDetailsUseCase;
    private readonly Mock<IUserRepository> userRepositoryMock = new();

    public UserGetDetailsUseCaseTests() =>
        this.userGetDetailsUseCase = new UserGetDetailsUseCase(this.userRepositoryMock.Object);

    [Fact]
    public async Task GetUserDetailsAsync()
    {
        var userId = new Guid("00aaba25-ffad-4564-bffc-57a3b85fc171");
        var user = new User
        {
            Id = userId,
            FirstName = "FirstName",
            LastName = "LastName"
        };
        var cancellationToken = CancellationToken.None;

        this.userRepositoryMock
            .Setup(userRepository => userRepository.FindOneAsync(userId, cancellationToken))
            .ReturnsAsync(user);

        var userDto = await this.userGetDetailsUseCase.GetUserDetailsAsync(userId, cancellationToken);

        userDto.Id.Should().Be(userId);
        userDto.FirstName.Should().Be(user.FirstName);
        userDto.LastName.Should().Be(user.LastName);
    }

    [Fact]
    public async Task GetUserDetailsAsyncThrowsUserNotFoundException()
    {
        var userId = new Guid("00aaba25-ffad-4564-bffc-57a3b85fc171");
        var cancellationToken = CancellationToken.None;

        this.userRepositoryMock
            .Setup(userRepository => userRepository.FindOneAsync(userId, cancellationToken))
            .ReturnsAsync((User?)null);

        await Invoking(() => this.userGetDetailsUseCase.GetUserDetailsAsync(userId, cancellationToken))
            .Should().ThrowAsync<UserNotFoundException>()
            .WithMessage($"Could not find user {userId}");
    }
}
