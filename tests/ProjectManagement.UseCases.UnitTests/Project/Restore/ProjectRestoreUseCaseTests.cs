namespace DotnetProjectManagement.ProjectManagement.UseCases.UnitTests.Project.Restore;

using Abstractions;
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
    private readonly Mock<IActivityRepository> activityRepositoryMock = new();
    private readonly Mock<ITransactionManager> transactionManagerMock = new();
    private readonly Mock<TimeProvider> timeProviderMock = new();
    private readonly Mock<ITransaction> transactionMock = new();

    public ProjectRestoreUseCaseTests() =>
        this.projectRestoreUseCase = new ProjectRestoreUseCase(
            this.projectRepositoryMock.Object,
            this.activityRepositoryMock.Object,
            this.transactionManagerMock.Object,
            this.timeProviderMock.Object,
            new NullLogger<ProjectRestoreUseCase>());

    [Fact]
    public async Task RestoreProjectAsyncAsManager()
    {
        var projectId = new Guid("00aaba25-ffad-4564-bffc-57a3b85fc171");
        var userId = new Guid("fd9f45e1-48f0-42ae-a390-2f4d1653451f");
        var actor = new Actor
        {
            UserId = userId,
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
            .Setup(projectRepository =>
                projectRepository.SaveAsync(Capture.In(capturedProjects), cancellationToken));

        var capturedActivities = new List<ProjectRestoredActivity>();
        this.activityRepositoryMock
            .Setup(activityRepository =>
                activityRepository.SaveAsync(Capture.In(capturedActivities), cancellationToken));

        var now = DateTimeOffset.FromUnixTimeSeconds(123);
        this.timeProviderMock
            .Setup(timeProvider => timeProvider.GetUtcNow())
            .Returns(() => now);

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

        capturedActivities.Should().SatisfyRespectively(capturedActivity =>
        {
            capturedActivity.UserId.Should().Be(userId);
            capturedActivity.Timestamp.Should().Be(now);
            capturedActivity.ProjectId.Should().Be(projectId);
        });

        this.transactionMock.Verify(transaction => transaction.CommitAsync(cancellationToken));
    }

    [Fact]
    public async Task RestoreProjectAsyncAsAdministrator()
    {
        var projectId = new Guid("00aaba25-ffad-4564-bffc-57a3b85fc171");
        var userId = new Guid("fd9f45e1-48f0-42ae-a390-2f4d1653451f");
        var actor = new Actor
        {
            UserId = userId,
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

        var capturedActivities = new List<ProjectRestoredActivity>();
        this.activityRepositoryMock
            .Setup(activityRepository =>
                activityRepository.SaveAsync(Capture.In(capturedActivities), cancellationToken));

        var now = DateTimeOffset.FromUnixTimeSeconds(123);
        this.timeProviderMock
            .Setup(timeProvider => timeProvider.GetUtcNow())
            .Returns(() => now);

        var projectDto = await this.projectRestoreUseCase.RestoreProjectAsync(actor, projectId, cancellationToken);

        this.projectRepositoryMock.Verify(projectRepository => projectRepository.SaveAsync(project, cancellationToken));

        projectDto.Id.Should().Be(projectId);
        projectDto.DisplayName.Should().Be("DisplayName");
        projectDto.Archived.Should().BeFalse();
        projectDto.Members.Should().BeEmpty();

        capturedActivities.Should().SatisfyRespectively(capturedActivity =>
        {
            capturedActivity.UserId.Should().Be(userId);
            capturedActivity.Timestamp.Should().Be(now);
            capturedActivity.ProjectId.Should().Be(projectId);
        });
    }

    [Fact]
    public async Task RestoreProjectAsyncThrowsProjectNotArchivedException()
    {
        var projectId = new Guid("00aaba25-ffad-4564-bffc-57a3b85fc171");
        var actor = new Actor
        {
            UserId = new Guid("fd9f45e1-48f0-42ae-a390-2f4d1653451f"),
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
