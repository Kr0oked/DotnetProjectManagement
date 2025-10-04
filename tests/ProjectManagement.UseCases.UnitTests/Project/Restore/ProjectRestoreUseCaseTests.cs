namespace DotnetProjectManagement.ProjectManagement.UseCases.UnitTests.Project.Restore;

using Abstractions;
using Domain.Actions;
using Domain.Entities;
using Exceptions;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using UseCases.DTOs;
using UseCases.Project.Restore;
using Xunit;
using static FluentAssertions.FluentActions;

public class ProjectRestoreUseCaseTests
{
    private readonly ProjectRestoreUseCase projectRestoreUseCase;
    private readonly Mock<IProjectRepository> projectRepositoryMock = new();
    private readonly Mock<ITransactionManager> transactionManagerMock = new();
    private readonly Mock<IMessageBroker> messageBrokerMock = new();
    private readonly Mock<ITransaction> transactionMock = new();

    public ProjectRestoreUseCaseTests() =>
        this.projectRestoreUseCase = new ProjectRestoreUseCase(
            this.projectRepositoryMock.Object,
            this.transactionManagerMock.Object,
            this.messageBrokerMock.Object,
            new NullLogger<ProjectRestoreUseCase>());

    [Fact]
    public async Task RestoreProjectAsyncAsManager()
    {
        var projectId = new Guid("00aaba25-ffad-4564-bffc-57a3b85fc171");
        var userId = new Guid("fd9f45e1-48f0-42ae-a390-2f4d1653451f");
        var actor = new Actor
        {
            UserId = userId,
            FirstName = "FirstName",
            LastName = "LastName",
            IsAdministrator = false
        };
        var cancellationToken = CancellationToken.None;

        this.transactionManagerMock
            .Setup(transactionManager => transactionManager.BeginTransactionAsync(cancellationToken))
            .ReturnsAsync(this.transactionMock.Object);

        var project = new Project
        {
            Id = projectId,
            DisplayName = "DisplayName",
            Archived = true,
            Members = new Dictionary<Guid, ProjectMemberRole> { { userId, ProjectMemberRole.Manager } }
        };

        this.projectRepositoryMock
            .Setup(projectRepository => projectRepository.FindOneAsync(projectId, cancellationToken))
            .ReturnsAsync(project);

        var capturedProjects = new List<Project>();
        this.projectRepositoryMock
            .Setup(projectRepository => projectRepository
                .SaveAsync(Capture.In(capturedProjects), ProjectAction.Restore, userId, cancellationToken));

        var projectDto = await this.projectRestoreUseCase.RestoreProjectAsync(actor, projectId, cancellationToken);

        projectDto.Id.Should().Be(projectId);
        projectDto.DisplayName.Should().Be("DisplayName");
        projectDto.Archived.Should().BeFalse();
        projectDto.Members.Should().Equal(new Dictionary<Guid, ProjectMemberRole>
        {
            { userId, ProjectMemberRole.Manager }
        });

        capturedProjects.Should().SatisfyRespectively(capturedProject =>
        {
            capturedProject.Should().Be(project);
            capturedProject.Archived.Should().BeFalse();
        });

        this.transactionMock.Verify(transaction => transaction.CommitAsync(cancellationToken));

        var capturedMessages = new List<ProjectActionMessage>();
        this.messageBrokerMock.Verify(messageBroker => messageBroker
            .Publish(Capture.In(capturedMessages), cancellationToken));
        capturedMessages.Should().SatisfyRespectively(message =>
        {
            message.ActorUserId.Should().Be(userId);
            message.Action.Should().Be(ProjectAction.Restore);
            message.Project.Should().BeEquivalentTo(projectDto);
        });
    }

    [Fact]
    public async Task RestoreProjectAsyncAsAdministrator()
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
        var cancellationToken = CancellationToken.None;

        this.transactionManagerMock
            .Setup(transactionManager => transactionManager.BeginTransactionAsync(cancellationToken))
            .ReturnsAsync(this.transactionMock.Object);

        var project = new Project
        {
            Id = projectId,
            DisplayName = "DisplayName",
            Archived = true,
            Members = []
        };

        this.projectRepositoryMock
            .Setup(projectRepository => projectRepository.FindOneAsync(projectId, cancellationToken))
            .ReturnsAsync(project);

        var projectDto = await this.projectRestoreUseCase.RestoreProjectAsync(actor, projectId, cancellationToken);

        this.projectRepositoryMock
            .Verify(projectRepository => projectRepository
                .SaveAsync(project, ProjectAction.Restore, userId, cancellationToken));

        projectDto.Id.Should().Be(projectId);
        projectDto.DisplayName.Should().Be("DisplayName");
        projectDto.Archived.Should().BeFalse();
        projectDto.Members.Should().BeEmpty();

        var capturedMessages = new List<ProjectActionMessage>();
        this.messageBrokerMock.Verify(messageBroker => messageBroker
            .Publish(Capture.In(capturedMessages), cancellationToken));
        capturedMessages.Should().SatisfyRespectively(message =>
        {
            message.ActorUserId.Should().Be(userId);
            message.Action.Should().Be(ProjectAction.Restore);
            message.Project.Should().BeEquivalentTo(projectDto);
        });
    }

    [Fact]
    public async Task RestoreProjectAsyncThrowsProjectNotArchivedException()
    {
        var projectId = new Guid("00aaba25-ffad-4564-bffc-57a3b85fc171");
        var actor = new Actor
        {
            UserId = new Guid("fd9f45e1-48f0-42ae-a390-2f4d1653451f"),
            FirstName = "FirstName",
            LastName = "LastName",
            IsAdministrator = true
        };
        var cancellationToken = CancellationToken.None;

        this.transactionManagerMock
            .Setup(transactionManager => transactionManager.BeginTransactionAsync(cancellationToken))
            .ReturnsAsync(this.transactionMock.Object);

        var project = new Project
        {
            Id = projectId,
            DisplayName = "DisplayName",
            Archived = false,
            Members = []
        };

        this.projectRepositoryMock
            .Setup(projectRepository => projectRepository.FindOneAsync(projectId, cancellationToken))
            .ReturnsAsync(project);

        await Invoking(() => this.projectRestoreUseCase.RestoreProjectAsync(actor, projectId, cancellationToken))
            .Should().ThrowAsync<ProjectNotArchivedException>()
            .WithMessage($"Project {projectId} is not archived");
    }

    [Fact]
    public async Task RestoreProjectAsyncThrowsManagerRequiredException()
    {
        var projectId = new Guid("00aaba25-ffad-4564-bffc-57a3b85fc171");
        var userId = new Guid("fd9f45e1-48f0-42ae-a390-2f4d1653451f");
        var actor = new Actor
        {
            UserId = userId,
            FirstName = "FirstName",
            LastName = "LastName",
            IsAdministrator = false
        };
        var cancellationToken = CancellationToken.None;

        this.transactionManagerMock
            .Setup(transactionManager => transactionManager.BeginTransactionAsync(cancellationToken))
            .ReturnsAsync(this.transactionMock.Object);

        var project = new Project
        {
            Id = projectId,
            DisplayName = "DisplayName",
            Archived = true,
            Members = []
        };

        this.projectRepositoryMock
            .Setup(projectRepository => projectRepository.FindOneAsync(projectId, cancellationToken))
            .ReturnsAsync(project);

        await Invoking(() => this.projectRestoreUseCase.RestoreProjectAsync(actor, projectId, cancellationToken))
            .Should().ThrowAsync<ManagerRequiredException>()
            .WithMessage($"User {userId} is not a manager in project {projectId}");
    }

    [Fact]
    public async Task RestoreProjectAsyncThrowsProjectNotFoundException()
    {
        var projectId = new Guid("00aaba25-ffad-4564-bffc-57a3b85fc171");
        var actor = new Actor
        {
            UserId = new Guid("fd9f45e1-48f0-42ae-a390-2f4d1653451f"),
            FirstName = "FirstName",
            LastName = "LastName",
            IsAdministrator = true
        };
        var cancellationToken = CancellationToken.None;

        this.transactionManagerMock
            .Setup(transactionManager => transactionManager.BeginTransactionAsync(cancellationToken))
            .ReturnsAsync(this.transactionMock.Object);

        this.projectRepositoryMock
            .Setup(projectRepository => projectRepository.FindOneAsync(projectId, cancellationToken))
            .ReturnsAsync((Project?)null);

        await Invoking(() => this.projectRestoreUseCase.RestoreProjectAsync(actor, projectId, cancellationToken))
            .Should().ThrowAsync<ProjectNotFoundException>()
            .WithMessage($"Could not find project {projectId}");
    }
}
