namespace DotnetProjectManagement.ProjectManagement.UseCases.UnitTests.Project.Create;

using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using Abstractions;
using Domain.Actions;
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
    private readonly ProjectCreateUseCase projectCreateUseCase;
    private readonly Mock<IProjectRepository> projectRepositoryMock = new();
    private readonly Mock<IUserRepository> userRepositoryMock = new();
    private readonly Mock<ITransactionManager> transactionManagerMock = new();
    private readonly Mock<IMessageBroker> messageBrokerMock = new();
    private readonly Mock<ITransaction> transactionMock = new();

    public ProjectCreateUseCaseTests() =>
        this.projectCreateUseCase = new ProjectCreateUseCase(
            this.projectRepositoryMock.Object,
            this.userRepositoryMock.Object,
            this.transactionManagerMock.Object,
            this.messageBrokerMock.Object,
            new NullLogger<ProjectCreateUseCase>());

    [Fact]
    public async Task CreateProjectAsync()
    {
        var projectId = new Guid("00aaba25-ffad-4564-bffc-57a3b85fc171");
        var userId = new Guid("fd9f45e1-48f0-42ae-a390-2f4d1653451f");
        var actor = new Actor
        {
            UserId = userId,
            FirstName = "FirstName",
            LastName = "LastName",
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
            .Setup(userRepository => userRepository.ExistsAsync(userId, cancellationToken))
            .ReturnsAsync(true);

        var capturedProjects = new List<Project>();
        this.projectRepositoryMock
            .Setup(projectRepository => projectRepository
                .SaveAsync(Capture.In(capturedProjects), ProjectAction.Create, userId, cancellationToken))
            .ReturnsAsync(new Project
            {
                Id = projectId,
                DisplayName = "DisplayName",
                Archived = false,
                Members = new Dictionary<Guid, ProjectMemberRole> { { userId, ProjectMemberRole.Manager } }
            });

        var projectDto = await this.projectCreateUseCase.CreateProjectAsync(actor, command, cancellationToken);

        projectDto.Id.Should().Be(projectId);
        projectDto.DisplayName.Should().Be("DisplayName");
        projectDto.Archived.Should().BeFalse();
        projectDto.Members.Should().Equal(new Dictionary<Guid, ProjectMemberRole>
        {
            { userId, ProjectMemberRole.Manager }
        });

        capturedProjects.Should().SatisfyRespectively(capturedProject =>
        {
            capturedProject.Id.Should().BeEmpty();
            capturedProject.DisplayName.Should().Be("DisplayName");
            capturedProject.Archived.Should().BeFalse();
            capturedProject.Members.Should().Equal(new Dictionary<Guid, ProjectMemberRole>
            {
                { userId, ProjectMemberRole.Manager }
            });
        });

        this.transactionMock.Verify(transaction => transaction.CommitAsync(cancellationToken));

        var capturedMessages = new List<ProjectActionMessage>();
        this.messageBrokerMock.Verify(messageBroker => messageBroker
            .Publish(Capture.In(capturedMessages), cancellationToken));
        capturedMessages.Should().SatisfyRespectively(message =>
        {
            message.ActorUserId.Should().Be(userId);
            message.Action.Should().Be(ProjectAction.Create);
            message.Project.Should().BeEquivalentTo(projectDto);
        });
    }

    [Fact]
    public async Task CreateProjectAsyncThrowsValidationException()
    {
        var actor = new Actor
        {
            UserId = new Guid("fd9f45e1-48f0-42ae-a390-2f4d1653451f"),
            FirstName = "FirstName",
            LastName = "LastName",
            IsAdministrator = true
        };
        var command = new ProjectCreateCommand
        {
            DisplayName = string.Concat(Enumerable.Repeat("a", 256)),
            Members = ImmutableDictionary<Guid, ProjectMemberRole>.Empty
        };
        var cancellationToken = CancellationToken.None;

        this.transactionManagerMock
            .Setup(transactionManager => transactionManager.BeginTransactionAsync(cancellationToken))
            .ReturnsAsync(this.transactionMock.Object);

        await Invoking(() => this.projectCreateUseCase.CreateProjectAsync(actor, command, cancellationToken))
            .Should().ThrowAsync<ValidationException>()
            .WithMessage("The field DisplayName must be a string with a maximum length of 255.");
    }

    [Fact]
    public async Task CreateProjectAsyncThrowsUserNotFoundException()
    {
        var userId = new Guid("fd9f45e1-48f0-42ae-a390-2f4d1653451f");
        var actor = new Actor
        {
            UserId = userId,
            FirstName = "FirstName",
            LastName = "LastName",
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
            .Setup(userRepository => userRepository.ExistsAsync(userId, cancellationToken))
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
            FirstName = "FirstName",
            LastName = "LastName",
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
