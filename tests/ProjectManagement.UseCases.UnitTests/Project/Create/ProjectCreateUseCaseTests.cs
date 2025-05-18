namespace DotnetProjectManagement.ProjectManagement.UseCases.UnitTests.Project.Create;

using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using Abstractions;
using Domain.Entities;
using Exceptions;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using UseCases.DTOs;
using UseCases.Project.Create;
using Xunit;
using static FluentAssertions.FluentActions;

public class ProjectCreateUseCaseTests
{
    private readonly Mock<IActivityRepository> activityRepositoryMock = new();
    private readonly ProjectCreateUseCase projectCreateUseCase;
    private readonly Mock<IProjectRepository> projectRepositoryMock = new();
    private readonly Mock<TimeProvider> timeProviderMock = new();
    private readonly Mock<ITransactionManager> transactionManagerMock = new();
    private readonly Mock<ITransaction> transactionMock = new();
    private readonly Mock<IUserRepository> userRepositoryMock = new();

    public ProjectCreateUseCaseTests() =>
        this.projectCreateUseCase = new ProjectCreateUseCase(
            this.projectRepositoryMock.Object,
            this.userRepositoryMock.Object,
            this.activityRepositoryMock.Object,
            this.transactionManagerMock.Object,
            this.timeProviderMock.Object,
            new NullLogger<ProjectCreateUseCase>());

    [Fact]
    public async Task CreateProjectAsync()
    {
        var userId = new Guid("fd9f45e1-48f0-42ae-a390-2f4d1653451f");
        var actor = new Actor
        {
            UserId = userId,
            IsAdministrator = true
        };
        var command = new ProjectCreateCommand
        {
            DisplayName = "DisplayName",
            Members = new Dictionary<Guid, ProjectMemberRole> { { userId, ProjectMemberRole.Manager } }
                .ToImmutableDictionary()
        };
        var cancellationToken = CancellationToken.None;

        this.transactionManagerMock
            .Setup(transactionManager => transactionManager.BeginTransactionAsync(cancellationToken))
            .ReturnsAsync(this.transactionMock.Object);

        this.userRepositoryMock
            .Setup(userRepository =>
                userRepository.ExistsAsync(userId, cancellationToken))
            .ReturnsAsync(true);

        var capturedProjects = new List<Project>();
        this.projectRepositoryMock
            .Setup(projectRepository =>
                projectRepository.SaveAsync(Capture.In(capturedProjects), cancellationToken));

        var capturedActivities = new List<ProjectCreatedActivity>();
        this.activityRepositoryMock
            .Setup(activityRepository =>
                activityRepository.SaveAsync(Capture.In(capturedActivities), cancellationToken));

        var now = DateTimeOffset.FromUnixTimeSeconds(123);
        this.timeProviderMock
            .Setup(timeProvider => timeProvider.GetUtcNow())
            .Returns(() => now);

        var projectDto = await this.projectCreateUseCase.CreateProjectAsync(actor, command, cancellationToken);

        projectDto.DisplayName.Should().Be("DisplayName");
        projectDto.Archived.Should().BeFalse();
        projectDto.Members.Should().Equal(new Dictionary<Guid, ProjectMemberRole>
        {
            { userId, ProjectMemberRole.Manager }
        });

        capturedProjects.Should().SatisfyRespectively(capturedProject =>
        {
            capturedProject.Id.Should().Be(projectDto.Id);
            capturedProject.DisplayName.Should().Be("DisplayName");
            capturedProject.Archived.Should().BeFalse();
            capturedProject.Members.Should().Equal(new Dictionary<Guid, ProjectMemberRole>
            {
                { userId, ProjectMemberRole.Manager }
            });
        });

        capturedActivities.Should().SatisfyRespectively(capturedActivity =>
        {
            capturedActivity.UserId.Should().Be(userId);
            capturedActivity.Timestamp.Should().Be(now);
            capturedActivity.ProjectId.Should().Be(projectDto.Id);
            capturedActivity.DisplayName.Should().Be("DisplayName");
            capturedActivity.Members.Should().Equal(new Dictionary<Guid, ProjectMemberRole>
            {
                { userId, ProjectMemberRole.Manager }
            });
        });

        this.transactionMock.Verify(transaction => transaction.CommitAsync(cancellationToken));
    }

    [Fact]
    public async Task CreateProjectAsyncThrowsValidationException()
    {
        var actor = new Actor
        {
            UserId = new Guid("fd9f45e1-48f0-42ae-a390-2f4d1653451f"),
            IsAdministrator = true
        };
        var command = new ProjectCreateCommand
        {
            DisplayName = "",
            Members = ImmutableDictionary<Guid, ProjectMemberRole>.Empty
        };
        var cancellationToken = CancellationToken.None;

        this.transactionManagerMock
            .Setup(transactionManager => transactionManager.BeginTransactionAsync(cancellationToken))
            .ReturnsAsync(this.transactionMock.Object);

        await Invoking(() => this.projectCreateUseCase.CreateProjectAsync(actor, command, cancellationToken))
            .Should().ThrowAsync<ValidationException>()
            .WithMessage("The DisplayName field is required.");
    }

    [Fact]
    public async Task CreateProjectAsyncThrowsUserNotFoundException()
    {
        var userId = new Guid("fd9f45e1-48f0-42ae-a390-2f4d1653451f");
        var actor = new Actor
        {
            UserId = userId,
            IsAdministrator = true
        };
        var command = new ProjectCreateCommand
        {
            DisplayName = "DisplayName",
            Members = new Dictionary<Guid, ProjectMemberRole> { { userId, ProjectMemberRole.Manager } }
                .ToImmutableDictionary()
        };
        var cancellationToken = CancellationToken.None;

        this.userRepositoryMock
            .Setup(userRepository =>
                userRepository.ExistsAsync(userId, cancellationToken))
            .ReturnsAsync(false);

        await Invoking(() => this.projectCreateUseCase.CreateProjectAsync(actor, command, cancellationToken))
            .Should().ThrowAsync<UserNotFoundException>()
            .WithMessage($"Could not find user {userId}");
    }

    [Fact]
    public async Task CreateProjectAsyncThrowsAdministratorRequiredException()
    {
        var userId = new Guid("fd9f45e1-48f0-42ae-a390-2f4d1653451f");
        var actor = new Actor
        {
            UserId = userId,
            IsAdministrator = false
        };
        var command = new ProjectCreateCommand
        {
            DisplayName = "DisplayName",
            Members = ImmutableDictionary<Guid, ProjectMemberRole>.Empty
        };

        await Invoking(() => this.projectCreateUseCase.CreateProjectAsync(actor, command, CancellationToken.None))
            .Should().ThrowAsync<AdministratorRequiredException>()
            .WithMessage($"User {userId} is not an administrator");
    }
}
