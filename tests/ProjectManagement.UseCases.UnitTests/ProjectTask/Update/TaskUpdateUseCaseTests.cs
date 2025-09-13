namespace DotnetProjectManagement.ProjectManagement.UseCases.UnitTests.ProjectTask.Update;

using System.ComponentModel.DataAnnotations;
using Abstractions;
using Domain.Entities;
using Exceptions;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using UseCases.DTOs;
using UseCases.ProjectTask.Update;
using Xunit;
using static FluentAssertions.FluentActions;

public class TaskUpdateUseCaseTests
{
    private readonly TaskUpdateUseCase taskUpdateUseCase;
    private readonly Mock<ITaskRepository> taskRepositoryMock = new();
    private readonly Mock<IProjectRepository> projectRepositoryMock = new();
    private readonly Mock<IUserRepository> userRepositoryMock = new();
    private readonly Mock<IActivityRepository> activityRepositoryMock = new();
    private readonly Mock<ITransactionManager> transactionManagerMock = new();
    private readonly Mock<TimeProvider> timeProviderMock = new();
    private readonly Mock<ITransaction> transactionMock = new();

    public TaskUpdateUseCaseTests() =>
        this.taskUpdateUseCase = new TaskUpdateUseCase(
            this.taskRepositoryMock.Object,
            this.projectRepositoryMock.Object,
            this.userRepositoryMock.Object,
            this.activityRepositoryMock.Object,
            this.transactionManagerMock.Object,
            this.timeProviderMock.Object,
            new NullLogger<TaskUpdateUseCase>());

    [Fact]
    public async Task UpdateTaskAsyncAsAdministrator()
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
        var command = new TaskUpdateCommand
        {
            TaskId = taskId,
            DisplayName = "NewDisplayName",
            Description = "NewDescription",
            Assignees = [userId]
        };
        var cancellationToken = CancellationToken.None;

        this.transactionManagerMock
            .Setup(transactionManager => transactionManager.BeginTransactionAsync(cancellationToken))
            .ReturnsAsync(this.transactionMock.Object);

        var task = new ProjectTask
        {
            Id = taskId,
            DisplayName = "OldDisplayName",
            Description = "OldDescription",
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

        this.userRepositoryMock
            .Setup(userRepository => userRepository.ExistsAsync(userId, cancellationToken))
            .ReturnsAsync(true);

        var capturedActivities = new List<TaskUpdatedActivity>();
        this.activityRepositoryMock
            .Setup(activityRepository =>
                activityRepository.SaveAsync(Capture.In(capturedActivities), cancellationToken));

        var now = DateTimeOffset.FromUnixTimeSeconds(123);
        this.timeProviderMock
            .Setup(timeProvider => timeProvider.GetUtcNow())
            .Returns(() => now);

        var taskDto = await this.taskUpdateUseCase.UpdateTaskAsync(actor, command, cancellationToken);

        taskDto.Id.Should().Be(taskId);
        taskDto.DisplayName.Should().Be("NewDisplayName");
        taskDto.Description.Should().Be("NewDescription");
        taskDto.Open.Should().BeFalse();
        taskDto.Assignees.Should().Equal(userId);

        capturedTasks.Should().SatisfyRespectively(capturedTask =>
        {
            capturedTask.Should().Be(task);
            capturedTask.Id.Should().Be(taskId);
            capturedTask.DisplayName.Should().Be("NewDisplayName");
            capturedTask.Description.Should().Be("NewDescription");
            capturedTask.Open.Should().BeFalse();
            capturedTask.Assignees.Should().Equal(userId);
            capturedTask.ProjectId.Should().Be(projectId);
        });

        capturedActivities.Should().SatisfyRespectively(capturedActivity =>
        {
            capturedActivity.UserId.Should().Be(userId);
            capturedActivity.Timestamp.Should().Be(now);
            capturedActivity.TaskId.Should().Be(taskId);
            capturedActivity.NewDisplayName.Should().Be("NewDisplayName");
            capturedActivity.OldDisplayName.Should().Be("OldDisplayName");
            capturedActivity.NewDescription.Should().Be("NewDescription");
            capturedActivity.OldDescription.Should().Be("OldDescription");
            capturedActivity.NewAssignees.Should().Equal(userId);
            capturedActivity.OldAssignees.Should().BeEmpty();
        });

        this.transactionMock.Verify(transaction => transaction.CommitAsync(cancellationToken));
    }

    [Fact]
    public async Task UpdateTaskAsyncAsManager()
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
        var command = new TaskUpdateCommand
        {
            TaskId = taskId,
            DisplayName = "NewDisplayName",
            Description = "NewDescription",
            Assignees = [userId]
        };
        var cancellationToken = CancellationToken.None;

        this.transactionManagerMock
            .Setup(transactionManager => transactionManager.BeginTransactionAsync(cancellationToken))
            .ReturnsAsync(this.transactionMock.Object);

        var task = new ProjectTask
        {
            Id = taskId,
            DisplayName = "OldDisplayName",
            Description = "OldDescription",
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

        this.userRepositoryMock
            .Setup(userRepository => userRepository.ExistsAsync(userId, cancellationToken))
            .ReturnsAsync(true);

        var capturedTasks = new List<ProjectTask>();
        this.taskRepositoryMock
            .Setup(taskRepository => taskRepository.SaveAsync(Capture.In(capturedTasks), cancellationToken));

        var capturedActivities = new List<TaskUpdatedActivity>();
        this.activityRepositoryMock
            .Setup(activityRepository =>
                activityRepository.SaveAsync(Capture.In(capturedActivities), cancellationToken));

        var now = DateTimeOffset.FromUnixTimeSeconds(123);
        this.timeProviderMock
            .Setup(timeProvider => timeProvider.GetUtcNow())
            .Returns(() => now);

        var taskDto = await this.taskUpdateUseCase.UpdateTaskAsync(actor, command, cancellationToken);

        taskDto.Id.Should().Be(taskId);
        taskDto.DisplayName.Should().Be("NewDisplayName");
        taskDto.Description.Should().Be("NewDescription");
        taskDto.Open.Should().BeFalse();
        taskDto.Assignees.Should().Equal(userId);

        capturedTasks.Should().SatisfyRespectively(capturedTask =>
        {
            capturedTask.Should().Be(task);
            capturedTask.Id.Should().Be(taskId);
            capturedTask.DisplayName.Should().Be("NewDisplayName");
            capturedTask.Description.Should().Be("NewDescription");
            capturedTask.Open.Should().BeFalse();
            capturedTask.Assignees.Should().Equal(userId);
            capturedTask.ProjectId.Should().Be(projectId);
        });

        capturedActivities.Should().SatisfyRespectively(capturedActivity =>
        {
            capturedActivity.UserId.Should().Be(userId);
            capturedActivity.Timestamp.Should().Be(now);
            capturedActivity.TaskId.Should().Be(taskId);
            capturedActivity.NewDisplayName.Should().Be("NewDisplayName");
            capturedActivity.OldDisplayName.Should().Be("OldDisplayName");
            capturedActivity.NewDescription.Should().Be("NewDescription");
            capturedActivity.OldDescription.Should().Be("OldDescription");
            capturedActivity.NewAssignees.Should().Equal(userId);
            capturedActivity.OldAssignees.Should().BeEmpty();
        });

        this.transactionMock.Verify(transaction => transaction.CommitAsync(cancellationToken));
    }

    [Fact]
    public async Task UpdateTaskAsyncThrowsUserNotFoundException()
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
        var command = new TaskUpdateCommand
        {
            TaskId = taskId,
            DisplayName = "DisplayName",
            Description = "NewDescription",
            Assignees = [userId]
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

        this.userRepositoryMock
            .Setup(userRepository => userRepository.ExistsAsync(userId, cancellationToken))
            .ReturnsAsync(false);

        await Invoking(() => this.taskUpdateUseCase.UpdateTaskAsync(actor, command, cancellationToken))
            .Should().ThrowAsync<UserNotFoundException>()
            .WithMessage($"Could not find user {userId}");
    }

    [Fact]
    public async Task UpdateTaskAsyncThrowsValidationException()
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
        var command = new TaskUpdateCommand
        {
            TaskId = taskId,
            DisplayName = string.Concat(Enumerable.Repeat("a", 256)),
            Description = "NewDescription",
            Assignees = []
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

        await Invoking(() => this.taskUpdateUseCase.UpdateTaskAsync(actor, command, cancellationToken))
            .Should().ThrowAsync<ValidationException>()
            .WithMessage("The field DisplayName must be a string with a maximum length of 255.");
    }

    [Fact]
    public async Task UpdateTaskAsyncThrowsManagerRequiredException()
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
        var command = new TaskUpdateCommand
        {
            TaskId = taskId,
            DisplayName = "NewDisplayName",
            Description = "NewDescription",
            Assignees = []
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
            Members = new Dictionary<Guid, ProjectMemberRole> { { userId, ProjectMemberRole.Worker } }
        };

        this.projectRepositoryMock
            .Setup(projectRepository => projectRepository.FindOneAsync(projectId, cancellationToken))
            .ReturnsAsync(project);

        await Invoking(() => this.taskUpdateUseCase.UpdateTaskAsync(actor, command, cancellationToken))
            .Should().ThrowAsync<ManagerRequiredException>()
            .WithMessage($"User {userId} is not a manager in project {projectId}");
    }

    [Fact]
    public async Task UpdateTaskAsyncThrowsProjectArchivedException()
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
        var command = new TaskUpdateCommand
        {
            TaskId = taskId,
            DisplayName = "NewDisplayName",
            Description = "NewDescription",
            Assignees = []
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
            Archived = true,
            Members = []
        };

        this.projectRepositoryMock
            .Setup(projectRepository => projectRepository.FindOneAsync(projectId, cancellationToken))
            .ReturnsAsync(project);

        await Invoking(() => this.taskUpdateUseCase.UpdateTaskAsync(actor, command, cancellationToken))
            .Should().ThrowAsync<ProjectArchivedException>()
            .WithMessage($"Project {projectId} is archived");
    }

    [Fact]
    public async Task UpdateTaskAsyncThrowsProjectNotFoundException()
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
        var command = new TaskUpdateCommand
        {
            TaskId = taskId,
            DisplayName = "NewDisplayName",
            Description = "NewDescription",
            Assignees = []
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

        this.projectRepositoryMock
            .Setup(projectRepository => projectRepository.FindOneAsync(projectId, cancellationToken))
            .ReturnsAsync((Project?)null);

        await Invoking(() => this.taskUpdateUseCase.UpdateTaskAsync(actor, command, cancellationToken))
            .Should().ThrowAsync<ProjectNotFoundException>()
            .WithMessage($"Could not find project {projectId}");
    }

    [Fact]
    public async Task UpdateTaskAsyncThrowsTaskNotFoundException()
    {
        var taskId = new Guid("1ce8aedc-04ed-410b-821d-5556c3a18ccc");
        var actor = new Actor
        {
            UserId = new Guid("fd9f45e1-48f0-42ae-a390-2f4d1653451f"),
            FirstName = "FirstName",
            LastName = "LastName",
            IsAdministrator = true
        };
        var command = new TaskUpdateCommand
        {
            TaskId = taskId,
            DisplayName = "NewDisplayName",
            Description = "NewDescription",
            Assignees = []
        };
        var cancellationToken = CancellationToken.None;

        this.transactionManagerMock
            .Setup(transactionManager => transactionManager.BeginTransactionAsync(cancellationToken))
            .ReturnsAsync(this.transactionMock.Object);

        this.taskRepositoryMock
            .Setup(taskRepository => taskRepository.FindOneAsync(taskId, cancellationToken))
            .ReturnsAsync((ProjectTask?)null);

        await Invoking(() => this.taskUpdateUseCase.UpdateTaskAsync(actor, command, cancellationToken))
            .Should().ThrowAsync<TaskNotFoundException>()
            .WithMessage($"Could not find task {taskId}");
    }
}
