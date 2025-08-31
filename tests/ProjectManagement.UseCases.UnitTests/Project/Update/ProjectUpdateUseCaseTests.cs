namespace DotnetProjectManagement.ProjectManagement.UseCases.UnitTests.Project.Update;

using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using Abstractions;
using Domain.Entities;
using Exceptions;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using UseCases.DTOs;
using UseCases.Project.Update;
using Xunit;
using static FluentAssertions.FluentActions;

public class ProjectUpdateUseCaseTests
{
    private readonly ProjectUpdateUseCase projectUpdateUseCase;
    private readonly Mock<IProjectRepository> projectRepositoryMock = new();
    private readonly Mock<IUserRepository> userRepositoryMock = new();
    private readonly Mock<IActivityRepository> activityRepositoryMock = new();
    private readonly Mock<ITransactionManager> transactionManagerMock = new();
    private readonly Mock<TimeProvider> timeProviderMock = new();
    private readonly Mock<ITransaction> transactionMock = new();

    public ProjectUpdateUseCaseTests() =>
        this.projectUpdateUseCase = new ProjectUpdateUseCase(
            this.projectRepositoryMock.Object,
            this.userRepositoryMock.Object,
            this.activityRepositoryMock.Object,
            this.transactionManagerMock.Object,
            this.timeProviderMock.Object,
            new NullLogger<ProjectUpdateUseCase>());

    [Fact]
    public async Task UpdateProjectAsyncAsAdministrator()
    {
        var projectId = new Guid("00aaba25-ffad-4564-bffc-57a3b85fc171");
        var userId = new Guid("fd9f45e1-48f0-42ae-a390-2f4d1653451f");
        var actor = new Actor
        {
            UserId = userId,
            IsAdministrator = true
        };
        var command = new ProjectUpdateCommand
        {
            ProjectId = projectId,
            DisplayName = "NewDisplayName",
            Members = new Dictionary<Guid, ProjectMemberRole> { { userId, ProjectMemberRole.Manager } }
                .ToImmutableDictionary()
        };
        var cancellationToken = CancellationToken.None;

        this.transactionManagerMock
            .Setup(transactionManager => transactionManager.BeginTransactionAsync(cancellationToken))
            .ReturnsAsync(this.transactionMock.Object);

        var project = new Project
        {
            Id = command.ProjectId,
            DisplayName = "OldDisplayName",
            Archived = false,
            Members = []
        };

        this.projectRepositoryMock
            .Setup(projectRepository => projectRepository.FindOneAsync(command.ProjectId, cancellationToken))
            .ReturnsAsync(project);

        this.userRepositoryMock
            .Setup(userRepository =>
                userRepository.ExistsAsync(userId, cancellationToken))
            .ReturnsAsync(true);

        var capturedProjects = new List<Project>();
        this.projectRepositoryMock
            .Setup(projectRepository =>
                projectRepository.SaveAsync(Capture.In(capturedProjects), cancellationToken));

        var capturedActivities = new List<ProjectUpdatedActivity>();
        this.activityRepositoryMock
            .Setup(activityRepository =>
                activityRepository.SaveAsync(Capture.In(capturedActivities), cancellationToken));

        var now = DateTimeOffset.FromUnixTimeSeconds(123);
        this.timeProviderMock
            .Setup(timeProvider => timeProvider.GetUtcNow())
            .Returns(() => now);

        var projectDto = await this.projectUpdateUseCase.UpdateProjectAsync(actor, command, cancellationToken);

        projectDto.Id.Should().Be(projectId);
        projectDto.DisplayName.Should().Be("NewDisplayName");
        projectDto.Archived.Should().BeFalse();
        projectDto.Members.Should().Equal(new Dictionary<Guid, ProjectMemberRole>
        {
            { userId, ProjectMemberRole.Manager }
        });

        capturedProjects.Should().SatisfyRespectively(capturedProject =>
        {
            capturedProject.Should().Be(project);
            capturedProject.Id.Should().Be(projectId);
            capturedProject.DisplayName.Should().Be("NewDisplayName");
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
            capturedActivity.ProjectId.Should().Be(projectId);
            capturedActivity.NewDisplayName.Should().Be("NewDisplayName");
            capturedActivity.OldDisplayName.Should().Be("OldDisplayName");
            capturedActivity.NewMembers.Should().Equal(new Dictionary<Guid, ProjectMemberRole>
            {
                { userId, ProjectMemberRole.Manager }
            });
            capturedActivity.OldMembers.Should().BeEmpty();
        });

        this.transactionMock.Verify(transaction => transaction.CommitAsync(cancellationToken));
    }

    [Fact]
    public async Task UpdateProjectAsyncAsManager()
    {
        var projectId = new Guid("00aaba25-ffad-4564-bffc-57a3b85fc171");
        var userId = new Guid("fd9f45e1-48f0-42ae-a390-2f4d1653451f");
        var actor = new Actor
        {
            UserId = userId,
            IsAdministrator = false
        };
        var command = new ProjectUpdateCommand
        {
            ProjectId = projectId,
            DisplayName = "NewDisplayName",
            Members = new Dictionary<Guid, ProjectMemberRole> { { userId, ProjectMemberRole.Guest } }
                .ToImmutableDictionary()
        };
        var cancellationToken = CancellationToken.None;

        this.transactionManagerMock
            .Setup(transactionManager => transactionManager.BeginTransactionAsync(cancellationToken))
            .ReturnsAsync(this.transactionMock.Object);

        var project = new Project
        {
            Id = command.ProjectId,
            DisplayName = "OldDisplayName",
            Archived = false,
            Members = new Dictionary<Guid, ProjectMemberRole> { { userId, ProjectMemberRole.Manager } }
        };

        this.projectRepositoryMock
            .Setup(projectRepository => projectRepository.FindOneAsync(command.ProjectId, cancellationToken))
            .ReturnsAsync(project);

        this.userRepositoryMock
            .Setup(userRepository =>
                userRepository.ExistsAsync(userId, cancellationToken))
            .ReturnsAsync(true);

        var capturedProjects = new List<Project>();
        this.projectRepositoryMock
            .Setup(projectRepository =>
                projectRepository.SaveAsync(Capture.In(capturedProjects), cancellationToken));

        var capturedActivities = new List<ProjectUpdatedActivity>();
        this.activityRepositoryMock
            .Setup(activityRepository =>
                activityRepository.SaveAsync(Capture.In(capturedActivities), cancellationToken));

        var now = DateTimeOffset.FromUnixTimeSeconds(123);
        this.timeProviderMock
            .Setup(timeProvider => timeProvider.GetUtcNow())
            .Returns(() => now);

        var projectDto = await this.projectUpdateUseCase.UpdateProjectAsync(actor, command, cancellationToken);

        projectDto.Id.Should().Be(projectId);
        projectDto.DisplayName.Should().Be("NewDisplayName");
        projectDto.Archived.Should().BeFalse();
        projectDto.Members.Should().Equal(new Dictionary<Guid, ProjectMemberRole>
        {
            { userId, ProjectMemberRole.Guest }
        });

        capturedProjects.Should().SatisfyRespectively(capturedProject =>
        {
            capturedProject.Should().Be(project);
            capturedProject.Id.Should().Be(projectId);
            capturedProject.DisplayName.Should().Be("NewDisplayName");
            capturedProject.Archived.Should().BeFalse();
            capturedProject.Members.Should().Equal(new Dictionary<Guid, ProjectMemberRole>
            {
                { userId, ProjectMemberRole.Guest }
            });
        });

        capturedActivities.Should().SatisfyRespectively(capturedActivity =>
        {
            capturedActivity.UserId.Should().Be(userId);
            capturedActivity.Timestamp.Should().Be(now);
            capturedActivity.ProjectId.Should().Be(projectId);
            capturedActivity.NewDisplayName.Should().Be("NewDisplayName");
            capturedActivity.OldDisplayName.Should().Be("OldDisplayName");
            capturedActivity.NewMembers.Should().Equal(new Dictionary<Guid, ProjectMemberRole>
            {
                { userId, ProjectMemberRole.Guest }
            });
            capturedActivity.OldMembers.Should().Equal(new Dictionary<Guid, ProjectMemberRole>
            {
                { userId, ProjectMemberRole.Manager }
            });
        });

        this.transactionMock.Verify(transaction => transaction.CommitAsync(cancellationToken));
    }

    [Fact]
    public async Task UpdateProjectAsyncThrowsValidationException()
    {
        var projectId = new Guid("00aaba25-ffad-4564-bffc-57a3b85fc171");
        var userId = new Guid("fd9f45e1-48f0-42ae-a390-2f4d1653451f");
        var actor = new Actor
        {
            UserId = userId,
            IsAdministrator = true
        };
        var command = new ProjectUpdateCommand
        {
            ProjectId = projectId,
            DisplayName = string.Concat(Enumerable.Repeat("a", 256)),
            Members = ImmutableDictionary<Guid, ProjectMemberRole>.Empty
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

        await Invoking(() => this.projectUpdateUseCase.UpdateProjectAsync(actor, command, cancellationToken))
            .Should().ThrowAsync<ValidationException>()
            .WithMessage("The field DisplayName must be a string with a maximum length of 255.");
    }

    [Fact]
    public async Task UpdateProjectAsyncThrowsUserNotFoundException()
    {
        var projectId = new Guid("00aaba25-ffad-4564-bffc-57a3b85fc171");
        var userId = new Guid("fd9f45e1-48f0-42ae-a390-2f4d1653451f");
        var actor = new Actor
        {
            UserId = userId,
            IsAdministrator = true
        };
        var command = new ProjectUpdateCommand
        {
            ProjectId = projectId,
            DisplayName = "DisplayName",
            Members = new Dictionary<Guid, ProjectMemberRole> { { userId, ProjectMemberRole.Manager } }
                .ToImmutableDictionary()
        };
        var cancellationToken = CancellationToken.None;

        this.userRepositoryMock
            .Setup(userRepository =>
                userRepository.ExistsAsync(userId, cancellationToken))
            .ReturnsAsync(false);

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

        await Invoking(() => this.projectUpdateUseCase.UpdateProjectAsync(actor, command, cancellationToken))
            .Should().ThrowAsync<UserNotFoundException>()
            .WithMessage($"Could not find user {userId}");
    }

    [Fact]
    public async Task UpdateProjectAsyncThrowsProjectArchivedException()
    {
        var projectId = new Guid("00aaba25-ffad-4564-bffc-57a3b85fc171");
        var userId = new Guid("fd9f45e1-48f0-42ae-a390-2f4d1653451f");
        var actor = new Actor
        {
            UserId = userId,
            IsAdministrator = true
        };
        var command = new ProjectUpdateCommand
        {
            ProjectId = projectId,
            DisplayName = "DisplayName",
            Members = ImmutableDictionary<Guid, ProjectMemberRole>.Empty
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

        await Invoking(() => this.projectUpdateUseCase.UpdateProjectAsync(actor, command, cancellationToken))
            .Should().ThrowAsync<ProjectArchivedException>()
            .WithMessage($"Project {projectId} is archived");
    }

    [Fact]
    public async Task UpdateProjectAsyncThrowsManagerRequiredException()
    {
        var projectId = new Guid("00aaba25-ffad-4564-bffc-57a3b85fc171");
        var userId = new Guid("fd9f45e1-48f0-42ae-a390-2f4d1653451f");
        var actor = new Actor
        {
            UserId = userId,
            IsAdministrator = false
        };
        var command = new ProjectUpdateCommand
        {
            ProjectId = projectId,
            DisplayName = "DisplayName",
            Members = ImmutableDictionary<Guid, ProjectMemberRole>.Empty
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

        await Invoking(() => this.projectUpdateUseCase.UpdateProjectAsync(actor, command, cancellationToken))
            .Should().ThrowAsync<ManagerRequiredException>()
            .WithMessage($"User {userId} is not a manager in project {projectId}");
    }

    [Fact]
    public async Task UpdateProjectAsyncThrowsProjectNotFoundException()
    {
        var projectId = new Guid("00aaba25-ffad-4564-bffc-57a3b85fc171");
        var userId = new Guid("fd9f45e1-48f0-42ae-a390-2f4d1653451f");
        var actor = new Actor
        {
            UserId = userId,
            IsAdministrator = true
        };
        var command = new ProjectUpdateCommand
        {
            ProjectId = projectId,
            DisplayName = "DisplayName",
            Members = ImmutableDictionary<Guid, ProjectMemberRole>.Empty
        };
        var cancellationToken = CancellationToken.None;

        this.transactionManagerMock
            .Setup(transactionManager => transactionManager.BeginTransactionAsync(cancellationToken))
            .ReturnsAsync(this.transactionMock.Object);

        this.projectRepositoryMock
            .Setup(projectRepository => projectRepository.FindOneAsync(projectId, cancellationToken))
            .ReturnsAsync((Project?)null);

        await Invoking(() => this.projectUpdateUseCase.UpdateProjectAsync(actor, command, cancellationToken))
            .Should().ThrowAsync<ProjectNotFoundException>()
            .WithMessage($"Could not find project {projectId}");
    }
}
