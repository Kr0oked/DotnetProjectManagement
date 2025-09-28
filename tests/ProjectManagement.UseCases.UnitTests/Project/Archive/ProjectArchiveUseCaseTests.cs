namespace DotnetProjectManagement.ProjectManagement.UseCases.UnitTests.Project.Archive;

using Abstractions;
using Domain.Actions;
using Domain.Entities;
using Exceptions;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using UseCases.DTOs;
using UseCases.Project.Archive;
using Xunit;
using static FluentAssertions.FluentActions;

public class ProjectArchiveUseCaseTests
{
    private readonly ProjectArchiveUseCase projectArchiveUseCase;
    private readonly Mock<IProjectRepository> projectRepositoryMock = new();
    private readonly Mock<ITransactionManager> transactionManagerMock = new();
    private readonly Mock<ITransaction> transactionMock = new();

    public ProjectArchiveUseCaseTests() =>
        this.projectArchiveUseCase = new ProjectArchiveUseCase(
            this.projectRepositoryMock.Object,
            this.transactionManagerMock.Object,
            new NullLogger<ProjectArchiveUseCase>());

    [Fact]
    public async Task ArchiveProjectAsyncAsManager()
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
            Archived = false,
            Members = new Dictionary<Guid, ProjectMemberRole> { { userId, ProjectMemberRole.Manager } }
        };

        this.projectRepositoryMock
            .Setup(projectRepository => projectRepository.FindOneAsync(projectId, cancellationToken))
            .ReturnsAsync(project);

        var capturedProjects = new List<Project>();
        this.projectRepositoryMock
            .Setup(projectRepository => projectRepository
                .SaveAsync(Capture.In(capturedProjects), ProjectAction.Archive, userId, cancellationToken));

        var projectDto = await this.projectArchiveUseCase.ArchiveProjectAsync(actor, projectId, cancellationToken);

        projectDto.Id.Should().Be(projectId);
        projectDto.DisplayName.Should().Be("DisplayName");
        projectDto.Archived.Should().BeTrue();
        projectDto.Members.Should().Equal(new Dictionary<Guid, ProjectMemberRole>
        {
            { userId, ProjectMemberRole.Manager }
        });

        capturedProjects.Should().SatisfyRespectively(capturedProject =>
        {
            capturedProject.Should().Be(project);
            capturedProject.Archived.Should().BeTrue();
        });

        this.transactionMock.Verify(transaction => transaction.CommitAsync(cancellationToken));
    }

    [Fact]
    public async Task ArchiveProjectAsyncAsAdministrator()
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
            Archived = false,
            Members = []
        };

        this.projectRepositoryMock
            .Setup(projectRepository => projectRepository.FindOneAsync(projectId, cancellationToken))
            .ReturnsAsync(project);

        var projectDto = await this.projectArchiveUseCase.ArchiveProjectAsync(actor, projectId, cancellationToken);

        this.projectRepositoryMock
            .Verify(projectRepository => projectRepository
                .SaveAsync(project, ProjectAction.Archive, userId, cancellationToken));

        projectDto.Id.Should().Be(projectId);
        projectDto.DisplayName.Should().Be("DisplayName");
        projectDto.Archived.Should().BeTrue();
        projectDto.Members.Should().BeEmpty();
    }

    [Fact]
    public async Task ArchiveProjectAsyncThrowsProjectAlreadyArchivedException()
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
            Archived = true,
            Members = []
        };

        this.projectRepositoryMock
            .Setup(projectRepository => projectRepository.FindOneAsync(projectId, cancellationToken))
            .ReturnsAsync(project);

        await Invoking(() => this.projectArchiveUseCase.ArchiveProjectAsync(actor, projectId, cancellationToken))
            .Should().ThrowAsync<ProjectArchivedException>()
            .WithMessage($"Project {projectId} is archived");
    }

    [Fact]
    public async Task ArchiveProjectAsyncThrowsManagerRequiredException()
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
            Archived = false,
            Members = []
        };

        this.projectRepositoryMock
            .Setup(projectRepository => projectRepository.FindOneAsync(projectId, cancellationToken))
            .ReturnsAsync(project);

        await Invoking(() => this.projectArchiveUseCase.ArchiveProjectAsync(actor, projectId, cancellationToken))
            .Should().ThrowAsync<ManagerRequiredException>()
            .WithMessage($"User {userId} is not a manager in project {projectId}");
    }

    [Fact]
    public async Task ArchiveProjectAsyncThrowsProjectNotFoundException()
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

        await Invoking(() => this.projectArchiveUseCase.ArchiveProjectAsync(actor, projectId, cancellationToken))
            .Should().ThrowAsync<ProjectNotFoundException>()
            .WithMessage($"Could not find project {projectId}");
    }
}
