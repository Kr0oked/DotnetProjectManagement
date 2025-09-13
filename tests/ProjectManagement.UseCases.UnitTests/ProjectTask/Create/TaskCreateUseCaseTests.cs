namespace DotnetProjectManagement.ProjectManagement.UseCases.UnitTests.ProjectTask.Create;

using System.ComponentModel.DataAnnotations;
using Abstractions;
using Domain.Entities;
using Exceptions;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using UseCases.DTOs;
using UseCases.ProjectTask.Create;
using Xunit;
using static FluentAssertions.FluentActions;

public class TaskCreateUseCaseTests
{
    private readonly TaskCreateUseCase taskCreateUseCase;
    private readonly Mock<ITaskRepository> taskRepositoryMock = new();
    private readonly Mock<IProjectRepository> projectRepositoryMock = new();
    private readonly Mock<IUserRepository> userRepositoryMock = new();
    private readonly Mock<IActivityRepository> activityRepositoryMock = new();
    private readonly Mock<ITransactionManager> transactionManagerMock = new();
    private readonly Mock<TimeProvider> timeProviderMock = new();
    private readonly Mock<ITransaction> transactionMock = new();

    public TaskCreateUseCaseTests() =>
        this.taskCreateUseCase = new TaskCreateUseCase(
            this.taskRepositoryMock.Object,
            this.projectRepositoryMock.Object,
            this.userRepositoryMock.Object,
            this.activityRepositoryMock.Object,
            this.transactionManagerMock.Object,
            this.timeProviderMock.Object,
            new NullLogger<TaskCreateUseCase>());

    [Fact]
    public async Task CreateTaskAsyncAsAdministrator()
    {
        var taskId = new Guid("1ce8aedc-04ed-410b-821d-5556c3a18ccc");
        var projectId = new Guid("00aaba25-ffad-4564-bffc-57a3b85fc171");
        var userId = new Guid("fd9f45e1-48f0-42ae-a390-2f4d1653451f");
        var actor = new Actor
        {
            UserId = userId,
            FirstName = "FirstName",
            LastName = "LastName",
            IsAdministrator = true
        };
        var command = new TaskCreateCommand
        {
            ProjectId = projectId,
            DisplayName = "DisplayName",
            Description = "Description",
            Assignees = [userId]
        };
        var cancellationToken = CancellationToken.None;

        this.transactionManagerMock
            .Setup(transactionManager => transactionManager.BeginTransactionAsync(cancellationToken))
            .ReturnsAsync(this.transactionMock.Object);

        var project = new Project
        {
            Id = projectId,
            DisplayName = "ProjectDisplayName",
            Archived = false,
            Members = []
        };

        this.projectRepositoryMock
            .Setup(projectRepository => projectRepository.FindOneAsync(projectId, cancellationToken))
            .ReturnsAsync(project);

        this.userRepositoryMock
            .Setup(userRepository => userRepository.ExistsAsync(userId, cancellationToken))
            .ReturnsAsync(true);

        var capturedTasks = new List<ProjectTask>();
        this.taskRepositoryMock
            .Setup(taskRepository => taskRepository.SaveAsync(Capture.In(capturedTasks), cancellationToken))
            .ReturnsAsync(new ProjectTask
            {
                Id = taskId,
                DisplayName = "DisplayName",
                Description = "Description",
                Open = true,
                Assignees = [userId],
                ProjectId = projectId
            });

        var capturedActivities = new List<TaskCreatedActivity>();
        this.activityRepositoryMock
            .Setup(activityRepository =>
                activityRepository.SaveAsync(Capture.In(capturedActivities), cancellationToken));

        var now = DateTimeOffset.FromUnixTimeSeconds(123);
        this.timeProviderMock
            .Setup(timeProvider => timeProvider.GetUtcNow())
            .Returns(() => now);

        var taskDto = await this.taskCreateUseCase.CreateTaskAsync(actor, command, cancellationToken);

        taskDto.Id.Should().Be(taskId);
        taskDto.DisplayName.Should().Be("DisplayName");
        taskDto.Description.Should().Be("Description");
        taskDto.Open.Should().BeTrue();
        taskDto.Assignees.Should().Equal(userId);

        capturedTasks.Should().SatisfyRespectively(capturedTask =>
        {
            capturedTask.Id.Should().BeEmpty();
            capturedTask.DisplayName.Should().Be("DisplayName");
            capturedTask.Description.Should().Be("Description");
            capturedTask.Open.Should().BeTrue();
            capturedTask.Assignees.Should().Equal(userId);
            capturedTask.ProjectId.Should().Be(projectId);
        });

        capturedActivities.Should().SatisfyRespectively(capturedActivity =>
        {
            capturedActivity.UserId.Should().Be(userId);
            capturedActivity.Timestamp.Should().Be(now);
            capturedActivity.TaskId.Should().Be(taskDto.Id);
            capturedActivity.DisplayName.Should().Be("DisplayName");
            capturedActivity.Description.Should().Be("Description");
            capturedActivity.Assignees.Should().Equal(userId);
        });

        this.transactionMock.Verify(transaction => transaction.CommitAsync(cancellationToken));
    }

    [Fact]
    public async Task CreateTaskAsyncAsManager()
    {
        var taskId = new Guid("1ce8aedc-04ed-410b-821d-5556c3a18ccc");
        var projectId = new Guid("00aaba25-ffad-4564-bffc-57a3b85fc171");
        var userId = new Guid("fd9f45e1-48f0-42ae-a390-2f4d1653451f");
        var actor = new Actor
        {
            UserId = userId,
            FirstName = "FirstName",
            LastName = "LastName",
            IsAdministrator = false
        };
        var command = new TaskCreateCommand
        {
            ProjectId = projectId,
            DisplayName = "DisplayName",
            Description = "Description",
            Assignees = [userId]
        };
        var cancellationToken = CancellationToken.None;

        this.transactionManagerMock
            .Setup(transactionManager => transactionManager.BeginTransactionAsync(cancellationToken))
            .ReturnsAsync(this.transactionMock.Object);

        var project = new Project
        {
            Id = projectId,
            DisplayName = "ProjectDisplayName",
            Archived = false,
            Members = new Dictionary<Guid, ProjectMemberRole> { { userId, ProjectMemberRole.Manager } }
        };

        this.projectRepositoryMock
            .Setup(projectRepository => projectRepository.FindOneAsync(projectId, cancellationToken))
            .ReturnsAsync(project);

        this.userRepositoryMock
            .Setup(userRepository => userRepository.ExistsAsync(userId, cancellationToken))
            .ReturnsAsync(true);

        var capturedTasks = new List<ProjectTask>();
        this.taskRepositoryMock
            .Setup(taskRepository => taskRepository.SaveAsync(Capture.In(capturedTasks), cancellationToken))
            .ReturnsAsync(new ProjectTask
            {
                Id = taskId,
                DisplayName = "DisplayName",
                Description = "Description",
                Open = true,
                Assignees = [userId],
                ProjectId = projectId
            });

        var capturedActivities = new List<TaskCreatedActivity>();
        this.activityRepositoryMock
            .Setup(activityRepository =>
                activityRepository.SaveAsync(Capture.In(capturedActivities), cancellationToken));

        var now = DateTimeOffset.FromUnixTimeSeconds(123);
        this.timeProviderMock
            .Setup(timeProvider => timeProvider.GetUtcNow())
            .Returns(() => now);

        var taskDto = await this.taskCreateUseCase.CreateTaskAsync(actor, command, cancellationToken);

        taskDto.Id.Should().Be(taskId);
        taskDto.DisplayName.Should().Be("DisplayName");
        taskDto.Description.Should().Be("Description");
        taskDto.Open.Should().BeTrue();
        taskDto.Assignees.Should().Equal(userId);

        capturedTasks.Should().SatisfyRespectively(capturedTask =>
        {
            capturedTask.Id.Should().BeEmpty();
            capturedTask.DisplayName.Should().Be("DisplayName");
            capturedTask.Description.Should().Be("Description");
            capturedTask.Open.Should().BeTrue();
            capturedTask.Assignees.Should().Equal(userId);
            capturedTask.ProjectId.Should().Be(projectId);
        });

        capturedActivities.Should().SatisfyRespectively(capturedActivity =>
        {
            capturedActivity.UserId.Should().Be(userId);
            capturedActivity.Timestamp.Should().Be(now);
            capturedActivity.TaskId.Should().Be(taskDto.Id);
            capturedActivity.DisplayName.Should().Be("DisplayName");
            capturedActivity.Description.Should().Be("Description");
            capturedActivity.Assignees.Should().Equal(userId);
        });

        this.transactionMock.Verify(transaction => transaction.CommitAsync(cancellationToken));
    }

    [Fact]
    public async Task CreateTaskAsyncThrowsUserNotFoundException()
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
        var command = new TaskCreateCommand
        {
            ProjectId = projectId,
            DisplayName = "DisplayName",
            Description = "NewDescription",
            Assignees = [userId]
        };
        var cancellationToken = CancellationToken.None;

        this.transactionManagerMock
            .Setup(transactionManager => transactionManager.BeginTransactionAsync(cancellationToken))
            .ReturnsAsync(this.transactionMock.Object);

        var project = new Project
        {
            Id = projectId,
            DisplayName = "ProjectDisplayName",
            Archived = false,
            Members = []
        };

        this.projectRepositoryMock
            .Setup(projectRepository => projectRepository.FindOneAsync(projectId, cancellationToken))
            .ReturnsAsync(project);

        this.userRepositoryMock
            .Setup(userRepository => userRepository.ExistsAsync(userId, cancellationToken))
            .ReturnsAsync(false);

        await Invoking(() => this.taskCreateUseCase.CreateTaskAsync(actor, command, cancellationToken))
            .Should().ThrowAsync<UserNotFoundException>()
            .WithMessage($"Could not find user {userId}");
    }

    [Fact]
    public async Task CreateTaskAsyncThrowsValidationException()
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
        var command = new TaskCreateCommand
        {
            ProjectId = projectId,
            DisplayName = string.Concat(Enumerable.Repeat("a", 256)),
            Description = "NewDescription",
            Assignees = []
        };
        var cancellationToken = CancellationToken.None;

        this.transactionManagerMock
            .Setup(transactionManager => transactionManager.BeginTransactionAsync(cancellationToken))
            .ReturnsAsync(this.transactionMock.Object);

        var project = new Project
        {
            Id = projectId,
            DisplayName = "ProjectDisplayName",
            Archived = false,
            Members = []
        };

        this.projectRepositoryMock
            .Setup(projectRepository => projectRepository.FindOneAsync(projectId, cancellationToken))
            .ReturnsAsync(project);

        await Invoking(() => this.taskCreateUseCase.CreateTaskAsync(actor, command, cancellationToken))
            .Should().ThrowAsync<ValidationException>()
            .WithMessage("The field DisplayName must be a string with a maximum length of 255.");
    }

    [Fact]
    public async Task CreateTaskAsyncThrowsManagerRequiredException()
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
        var command = new TaskCreateCommand
        {
            ProjectId = projectId,
            DisplayName = "NewDisplayName",
            Description = "NewDescription",
            Assignees = []
        };
        var cancellationToken = CancellationToken.None;

        this.transactionManagerMock
            .Setup(transactionManager => transactionManager.BeginTransactionAsync(cancellationToken))
            .ReturnsAsync(this.transactionMock.Object);

        var project = new Project
        {
            Id = projectId,
            DisplayName = "ProjectDisplayName",
            Archived = false,
            Members = new Dictionary<Guid, ProjectMemberRole> { { userId, ProjectMemberRole.Worker } }
        };

        this.projectRepositoryMock
            .Setup(projectRepository => projectRepository.FindOneAsync(projectId, cancellationToken))
            .ReturnsAsync(project);

        await Invoking(() => this.taskCreateUseCase.CreateTaskAsync(actor, command, cancellationToken))
            .Should().ThrowAsync<ManagerRequiredException>()
            .WithMessage($"User {userId} is not a manager in project {projectId}");
    }

    [Fact]
    public async Task CreateTaskAsyncThrowsProjectArchivedException()
    {
        var projectId = new Guid("00aaba25-ffad-4564-bffc-57a3b85fc171");
        var actor = new Actor
        {
            UserId = new Guid("fd9f45e1-48f0-42ae-a390-2f4d1653451f"),
            FirstName = "FirstName",
            LastName = "LastName",
            IsAdministrator = true
        };
        var command = new TaskCreateCommand
        {
            ProjectId = projectId,
            DisplayName = "NewDisplayName",
            Description = "NewDescription",
            Assignees = []
        };
        var cancellationToken = CancellationToken.None;

        this.transactionManagerMock
            .Setup(transactionManager => transactionManager.BeginTransactionAsync(cancellationToken))
            .ReturnsAsync(this.transactionMock.Object);

        var project = new Project
        {
            Id = projectId,
            DisplayName = "ProjectDisplayName",
            Archived = true,
            Members = []
        };

        this.projectRepositoryMock
            .Setup(projectRepository => projectRepository.FindOneAsync(projectId, cancellationToken))
            .ReturnsAsync(project);

        await Invoking(() => this.taskCreateUseCase.CreateTaskAsync(actor, command, cancellationToken))
            .Should().ThrowAsync<ProjectArchivedException>()
            .WithMessage($"Project {projectId} is archived");
    }

    [Fact]
    public async Task UpdateTaskAsyncThrowsProjectNotFoundException()
    {
        var projectId = new Guid("00aaba25-ffad-4564-bffc-57a3b85fc171");
        var actor = new Actor
        {
            UserId = new Guid("fd9f45e1-48f0-42ae-a390-2f4d1653451f"),
            FirstName = "FirstName",
            LastName = "LastName",
            IsAdministrator = true
        };
        var command = new TaskCreateCommand
        {
            ProjectId = projectId,
            DisplayName = "NewDisplayName",
            Description = "NewDescription",
            Assignees = []
        };
        var cancellationToken = CancellationToken.None;

        this.transactionManagerMock
            .Setup(transactionManager => transactionManager.BeginTransactionAsync(cancellationToken))
            .ReturnsAsync(this.transactionMock.Object);

        this.projectRepositoryMock
            .Setup(projectRepository => projectRepository.FindOneAsync(projectId, cancellationToken))
            .ReturnsAsync((Project?)null);

        await Invoking(() => this.taskCreateUseCase.CreateTaskAsync(actor, command, cancellationToken))
            .Should().ThrowAsync<ProjectNotFoundException>()
            .WithMessage($"Could not find project {projectId}");
    }
}
