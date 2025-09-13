namespace DotnetProjectManagement.ProjectManagement.UseCases.UnitTests.ProjectTask.Reopen;

using Abstractions;
using Domain.Entities;
using Exceptions;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using UseCases.DTOs;
using UseCases.ProjectTask.Reopen;
using Xunit;
using static FluentAssertions.FluentActions;

public class TaskReopenUseCaseTests
{
    private readonly TaskReopenUseCase taskReopenUseCase;
    private readonly Mock<ITaskRepository> taskRepositoryMock = new();
    private readonly Mock<IProjectRepository> projectRepositoryMock = new();
    private readonly Mock<IActivityRepository> activityRepositoryMock = new();
    private readonly Mock<ITransactionManager> transactionManagerMock = new();
    private readonly Mock<TimeProvider> timeProviderMock = new();
    private readonly Mock<ITransaction> transactionMock = new();

    public TaskReopenUseCaseTests() =>
        this.taskReopenUseCase = new TaskReopenUseCase(
            this.taskRepositoryMock.Object,
            this.projectRepositoryMock.Object,
            this.activityRepositoryMock.Object,
            this.transactionManagerMock.Object,
            this.timeProviderMock.Object,
            new NullLogger<TaskReopenUseCase>());

    [Fact]
    public async Task ReopenTaskAsyncAsAssignee()
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
        var cancellationToken = CancellationToken.None;

        this.transactionManagerMock
            .Setup(transactionManager => transactionManager.BeginTransactionAsync(cancellationToken))
            .ReturnsAsync(this.transactionMock.Object);

        var task = new ProjectTask
        {
            Id = taskId,
            DisplayName = "DisplayName",
            Description = "Description",
            Open = false,
            Assignees = [userId],
            ProjectId = projectId
        };

        this.taskRepositoryMock
            .Setup(taskRepository => taskRepository.FindOneAsync(taskId, cancellationToken))
            .ReturnsAsync(task);

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

        var capturedTasks = new List<ProjectTask>();
        this.taskRepositoryMock
            .Setup(taskRepository => taskRepository.SaveAsync(Capture.In(capturedTasks), cancellationToken));

        var capturedActivities = new List<TaskReopenedActivity>();
        this.activityRepositoryMock
            .Setup(activityRepository =>
                activityRepository.SaveAsync(Capture.In(capturedActivities), cancellationToken));

        var now = DateTimeOffset.FromUnixTimeSeconds(123);
        this.timeProviderMock
            .Setup(timeProvider => timeProvider.GetUtcNow())
            .Returns(() => now);

        var taskDto = await this.taskReopenUseCase.ReopenTaskAsync(actor, taskId, cancellationToken);

        taskDto.Id.Should().Be(taskId);
        taskDto.DisplayName.Should().Be("DisplayName");
        taskDto.Description.Should().Be("Description");
        taskDto.Open.Should().BeTrue();
        taskDto.Assignees.Should().Equal(userId);

        capturedTasks.Should().SatisfyRespectively(capturedTask =>
        {
            capturedTask.Should().Be(task);
            capturedTask.Open.Should().BeTrue();
        });

        capturedActivities.Should().SatisfyRespectively(capturedActivity =>
        {
            capturedActivity.UserId.Should().Be(userId);
            capturedActivity.Timestamp.Should().Be(now);
            capturedActivity.TaskId.Should().Be(taskId);
        });

        this.transactionMock.Verify(transaction => transaction.CommitAsync(cancellationToken));
    }

    [Fact]
    public async Task ReopenTaskAsyncAsManager()
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
        var cancellationToken = CancellationToken.None;

        this.transactionManagerMock
            .Setup(transactionManager => transactionManager.BeginTransactionAsync(cancellationToken))
            .ReturnsAsync(this.transactionMock.Object);

        var task = new ProjectTask
        {
            Id = taskId,
            DisplayName = "DisplayName",
            Description = "Description",
            Open = false,
            Assignees = [],
            ProjectId = projectId
        };

        this.taskRepositoryMock
            .Setup(taskRepository => taskRepository.FindOneAsync(taskId, cancellationToken))
            .ReturnsAsync(task);

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

        var capturedTasks = new List<ProjectTask>();
        this.taskRepositoryMock
            .Setup(taskRepository => taskRepository.SaveAsync(Capture.In(capturedTasks), cancellationToken));

        var capturedActivities = new List<TaskReopenedActivity>();
        this.activityRepositoryMock
            .Setup(activityRepository =>
                activityRepository.SaveAsync(Capture.In(capturedActivities), cancellationToken));

        var now = DateTimeOffset.FromUnixTimeSeconds(123);
        this.timeProviderMock
            .Setup(timeProvider => timeProvider.GetUtcNow())
            .Returns(() => now);

        var taskDto = await this.taskReopenUseCase.ReopenTaskAsync(actor, taskId, cancellationToken);

        taskDto.Id.Should().Be(taskId);
        taskDto.DisplayName.Should().Be("DisplayName");
        taskDto.Description.Should().Be("Description");
        taskDto.Open.Should().BeTrue();
        taskDto.Assignees.Should().BeEmpty();

        capturedTasks.Should().SatisfyRespectively(capturedTask =>
        {
            capturedTask.Should().Be(task);
            capturedTask.Open.Should().BeTrue();
        });

        capturedActivities.Should().SatisfyRespectively(capturedActivity =>
        {
            capturedActivity.UserId.Should().Be(userId);
            capturedActivity.Timestamp.Should().Be(now);
            capturedActivity.TaskId.Should().Be(taskId);
        });

        this.transactionMock.Verify(transaction => transaction.CommitAsync(cancellationToken));
    }

    [Fact]
    public async Task ReopenTaskAsyncAsAdministrator()
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
        var cancellationToken = CancellationToken.None;

        this.transactionManagerMock
            .Setup(transactionManager => transactionManager.BeginTransactionAsync(cancellationToken))
            .ReturnsAsync(this.transactionMock.Object);

        var task = new ProjectTask
        {
            Id = taskId,
            DisplayName = "DisplayName",
            Description = "Description",
            Open = false,
            Assignees = [],
            ProjectId = projectId
        };

        this.taskRepositoryMock
            .Setup(taskRepository => taskRepository.FindOneAsync(taskId, cancellationToken))
            .ReturnsAsync(task);

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

        var capturedTasks = new List<ProjectTask>();
        this.taskRepositoryMock
            .Setup(taskRepository => taskRepository.SaveAsync(Capture.In(capturedTasks), cancellationToken));

        var capturedActivities = new List<TaskReopenedActivity>();
        this.activityRepositoryMock
            .Setup(activityRepository =>
                activityRepository.SaveAsync(Capture.In(capturedActivities), cancellationToken));

        var now = DateTimeOffset.FromUnixTimeSeconds(123);
        this.timeProviderMock
            .Setup(timeProvider => timeProvider.GetUtcNow())
            .Returns(() => now);

        var taskDto = await this.taskReopenUseCase.ReopenTaskAsync(actor, taskId, cancellationToken);

        taskDto.Id.Should().Be(taskId);
        taskDto.DisplayName.Should().Be("DisplayName");
        taskDto.Description.Should().Be("Description");
        taskDto.Open.Should().BeTrue();
        taskDto.Assignees.Should().BeEmpty();

        capturedTasks.Should().SatisfyRespectively(capturedTask =>
        {
            capturedTask.Should().Be(task);
            capturedTask.Open.Should().BeTrue();
        });

        capturedActivities.Should().SatisfyRespectively(capturedActivity =>
        {
            capturedActivity.UserId.Should().Be(userId);
            capturedActivity.Timestamp.Should().Be(now);
            capturedActivity.TaskId.Should().Be(taskId);
        });

        this.transactionMock.Verify(transaction => transaction.CommitAsync(cancellationToken));
    }

    [Fact]
    public async Task ReopenTaskAsyncThrowsTaskOpenException()
    {
        var taskId = new Guid("1ce8aedc-04ed-410b-821d-5556c3a18ccc");
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

        var task = new ProjectTask
        {
            Id = taskId,
            DisplayName = "DisplayName",
            Description = "Description",
            Open = true,
            Assignees = [],
            ProjectId = projectId
        };

        this.taskRepositoryMock
            .Setup(taskRepository => taskRepository.FindOneAsync(taskId, cancellationToken))
            .ReturnsAsync(task);

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

        await Invoking(() => this.taskReopenUseCase.ReopenTaskAsync(actor, taskId, cancellationToken))
            .Should().ThrowAsync<TaskOpenException>()
            .WithMessage($"Task {taskId} is open");
    }

    [Fact]
    public async Task ReopenTaskAsyncThrowsManagerRequiredException()
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
        var cancellationToken = CancellationToken.None;

        this.transactionManagerMock
            .Setup(transactionManager => transactionManager.BeginTransactionAsync(cancellationToken))
            .ReturnsAsync(this.transactionMock.Object);

        var task = new ProjectTask
        {
            Id = taskId,
            DisplayName = "DisplayName",
            Description = "Description",
            Open = false,
            Assignees = [],
            ProjectId = projectId
        };

        this.taskRepositoryMock
            .Setup(taskRepository => taskRepository.FindOneAsync(taskId, cancellationToken))
            .ReturnsAsync(task);

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

        await Invoking(() => this.taskReopenUseCase.ReopenTaskAsync(actor, taskId, cancellationToken))
            .Should().ThrowAsync<ManagerRequiredException>()
            .WithMessage($"User {userId} is not a manager in project {projectId}");
    }

    [Fact]
    public async Task ReopenTaskAsyncThrowsProjectArchivedException()
    {
        var taskId = new Guid("1ce8aedc-04ed-410b-821d-5556c3a18ccc");
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

        var task = new ProjectTask
        {
            Id = taskId,
            DisplayName = "DisplayName",
            Description = "Description",
            Open = false,
            Assignees = [],
            ProjectId = projectId
        };

        this.taskRepositoryMock
            .Setup(taskRepository => taskRepository.FindOneAsync(taskId, cancellationToken))
            .ReturnsAsync(task);

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

        await Invoking(() => this.taskReopenUseCase.ReopenTaskAsync(actor, taskId, cancellationToken))
            .Should().ThrowAsync<ProjectArchivedException>()
            .WithMessage($"Project {projectId} is archived");
    }

    [Fact]
    public async Task ReopenTaskAsyncThrowsProjectNotFoundException()
    {
        var taskId = new Guid("1ce8aedc-04ed-410b-821d-5556c3a18ccc");
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

        var task = new ProjectTask
        {
            Id = taskId,
            DisplayName = "DisplayName",
            Description = "Description",
            Open = false,
            Assignees = [],
            ProjectId = projectId
        };

        this.taskRepositoryMock
            .Setup(taskRepository => taskRepository.FindOneAsync(taskId, cancellationToken))
            .ReturnsAsync(task);

        this.projectRepositoryMock
            .Setup(projectRepository => projectRepository.FindOneAsync(projectId, cancellationToken))
            .ReturnsAsync((Project?)null);

        await Invoking(() => this.taskReopenUseCase.ReopenTaskAsync(actor, taskId, cancellationToken))
            .Should().ThrowAsync<ProjectNotFoundException>()
            .WithMessage($"Could not find project {projectId}");
    }

    [Fact]
    public async Task ReopenTaskAsyncThrowsTaskNotFoundException()
    {
        var taskId = new Guid("1ce8aedc-04ed-410b-821d-5556c3a18ccc");
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

        this.taskRepositoryMock
            .Setup(taskRepository => taskRepository.FindOneAsync(taskId, cancellationToken))
            .ReturnsAsync((ProjectTask?)null);

        await Invoking(() => this.taskReopenUseCase.ReopenTaskAsync(actor, taskId, cancellationToken))
            .Should().ThrowAsync<TaskNotFoundException>()
            .WithMessage($"Could not find task {taskId}");
    }
}
