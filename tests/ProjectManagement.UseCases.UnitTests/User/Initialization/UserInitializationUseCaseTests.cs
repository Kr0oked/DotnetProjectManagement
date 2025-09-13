namespace DotnetProjectManagement.ProjectManagement.UseCases.UnitTests.User.Initialization;

using System.ComponentModel.DataAnnotations;
using Abstractions;
using Domain.Entities;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using UseCases.DTOs;
using UseCases.User.Initialization;
using Xunit;
using static FluentAssertions.FluentActions;

public class UserInitializationUseCaseTests
{
    private readonly UserInitializationUseCase userInitializationUseCase;
    private readonly Mock<IUserRepository> userRepositoryMock = new();

    public UserInitializationUseCaseTests() => this.userInitializationUseCase =
        new UserInitializationUseCase(this.userRepositoryMock.Object, new NullLogger<UserInitializationUseCase>());

    [Fact]
    public async Task InitializeUserAsyncCreatesMissingUser()
    {
        var userId = new Guid("fd9f45e1-48f0-42ae-a390-2f4d1653451f");
        var actor = new Actor
        {
            UserId = userId,
            FirstName = "FirstName",
            LastName = "LastName",
            IsAdministrator = false
        };
        var cancellationToken = CancellationToken.None;

        this.userRepositoryMock
            .Setup(userRepository => userRepository.ExistsAsync(userId, cancellationToken))
            .ReturnsAsync(false);

        var capturedUsers = new List<User>();
        this.userRepositoryMock
            .Setup(userRepository => userRepository.SaveAsync(Capture.In(capturedUsers), cancellationToken));

        await this.userInitializationUseCase.InitializeUserAsync(actor, cancellationToken);

        capturedUsers.Should().SatisfyRespectively(capturedUser =>
        {
            capturedUser.Id.Should().Be(userId);
            capturedUser.FirstName.Should().Be("FirstName");
            capturedUser.LastName.Should().Be("LastName");
        });
    }

    [Fact]
    public async Task InitializeUserAsyncDoesNothingWhenUserAlreadyExists()
    {
        var userId = new Guid("fd9f45e1-48f0-42ae-a390-2f4d1653451f");
        var actor = new Actor
        {
            UserId = userId,
            FirstName = "FirstName",
            LastName = "LastName",
            IsAdministrator = false
        };
        var cancellationToken = CancellationToken.None;

        this.userRepositoryMock
            .Setup(userRepository => userRepository.ExistsAsync(userId, cancellationToken))
            .ReturnsAsync(true);

        await this.userInitializationUseCase.InitializeUserAsync(actor, cancellationToken);

        this.userRepositoryMock.Verify(userRepository => userRepository.ExistsAsync(userId, cancellationToken));
        this.userRepositoryMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task InitializeUserAsyncThrowsValidationException()
    {
        var userId = new Guid("fd9f45e1-48f0-42ae-a390-2f4d1653451f");
        var actor = new Actor
        {
            UserId = userId,
            FirstName = string.Concat(Enumerable.Repeat("a", 256)),
            LastName = "LastName",
            IsAdministrator = false
        };
        var cancellationToken = CancellationToken.None;

        this.userRepositoryMock
            .Setup(userRepository => userRepository.ExistsAsync(userId, cancellationToken))
            .ReturnsAsync(false);

        await Invoking(() => this.userInitializationUseCase.InitializeUserAsync(actor, cancellationToken))
            .Should().ThrowAsync<ValidationException>()
            .WithMessage("The field FirstName must be a string with a maximum length of 255.");
    }
}
