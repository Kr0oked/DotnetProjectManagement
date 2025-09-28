namespace DotnetProjectManagement.ProjectManagement.UseCases.UnitTests.ProjectTask.Close;

using Abstractions;
using Domain.Actions;
using Domain.Entities;
using Exceptions;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using UseCases.DTOs;
using UseCases.ProjectTask.Close;
using Xunit;
using static FluentAssertions.FluentActions;

public class TaskCloseUseCaseTests
{
    private readonly TaskCloseUseCase taskCloseUseCase;
    private readonly Mock<ITaskRepository> taskRepositoryMock = new();
    private readonly Mock<IProjectRepository> projectRepositoryMock = new();
    private readonly Mock<ITransactionManager> transactionManagerMock = new();
    private readonly Mock<ITransaction> transactionMock = new();

    public TaskCloseUseCaseTests() =>
        this.taskCloseUseCase = new TaskCloseUseCase(
            this.taskRepositoryMock.Object,
            this.projectRepositoryMock.Object,
            this.transactionManagerMock.Object,
            new NullLogger<TaskCloseUseCase>());

    [Fact]
    public async Task CloseTaskAsyncAsAssignee()
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
            Open = true,
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
            .Setup(taskRepository => taskRepository
                .SaveAsync(Capture.In(capturedTasks), TaskAction.Close, userId, cancellationToken));

        var taskDto = await this.taskCloseUseCase.CloseTaskAsync(actor, taskId, cancellationToken);

        taskDto.Id.Should().Be(taskId);
        taskDto.DisplayName.Should().Be("DisplayName");
        taskDto.Description.Should().Be("Description");
        taskDto.Open.Should().BeFalse();
        taskDto.Assignees.Should().Equal(userId);

        capturedTasks.Should().SatisfyRespectively(capturedTask =>
        {
            capturedTask.Should().Be(task);
            capturedTask.Open.Should().BeFalse();
        });

        this.transactionMock.Verify(transaction => transaction.CommitAsync(cancellationToken));
    }

    [Fact]
    public async Task CloseTaskAsyncAsManager()
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
            Members = new Dictionary<Guid, ProjectMemberRole> { { userId, ProjectMemberRole.Manager } }
        };

        this.projectRepositoryMock
            .Setup(projectRepository => projectRepository.FindOneAsync(projectId, cancellationToken))
            .ReturnsAsync(project);

        var capturedTasks = new List<ProjectTask>();
        this.taskRepositoryMock
            .Setup(taskRepository => taskRepository
                .SaveAsync(Capture.In(capturedTasks), TaskAction.Close, userId, cancellationToken));

        var taskDto = await this.taskCloseUseCase.CloseTaskAsync(actor, taskId, cancellationToken);

        taskDto.Id.Should().Be(taskId);
        taskDto.DisplayName.Should().Be("DisplayName");
        taskDto.Description.Should().Be("Description");
        taskDto.Open.Should().BeFalse();
        taskDto.Assignees.Should().BeEmpty();

        capturedTasks.Should().SatisfyRespectively(capturedTask =>
        {
            capturedTask.Should().Be(task);
            capturedTask.Open.Should().BeFalse();
        });

        this.transactionMock.Verify(transaction => transaction.CommitAsync(cancellationToken));
    }

    [Fact]
    public async Task CloseTaskAsyncAsAdministrator()
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

        var capturedTasks = new List<ProjectTask>();
        this.taskRepositoryMock
            .Setup(taskRepository => taskRepository
                .SaveAsync(Capture.In(capturedTasks), TaskAction.Close, userId, cancellationToken));

        var taskDto = await this.taskCloseUseCase.CloseTaskAsync(actor, taskId, cancellationToken);

        taskDto.Id.Should().Be(taskId);
        taskDto.DisplayName.Should().Be("DisplayName");
        taskDto.Description.Should().Be("Description");
        taskDto.Open.Should().BeFalse();
        taskDto.Assignees.Should().BeEmpty();

        capturedTasks.Should().SatisfyRespectively(capturedTask =>
        {
            capturedTask.Should().Be(task);
            capturedTask.Open.Should().BeFalse();
        });

        this.transactionMock.Verify(transaction => transaction.CommitAsync(cancellationToken));
    }

    [Fact]
    public async Task CloseTaskAsyncThrowsTaskClosedException()
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
            Archived = false,
            Members = []
        };

        this.projectRepositoryMock
            .Setup(projectRepository => projectRepository.FindOneAsync(projectId, cancellationToken))
            .ReturnsAsync(project);

        await Invoking(() => this.taskCloseUseCase.CloseTaskAsync(actor, taskId, cancellationToken))
            .Should().ThrowAsync<TaskClosedException>()
            .WithMessage($"Task {taskId} is closed");
    }

    [Fact]
    public async Task CloseTaskAsyncThrowsManagerRequiredException()
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

        await Invoking(() => this.taskCloseUseCase.CloseTaskAsync(actor, taskId, cancellationToken))
            .Should().ThrowAsync<ManagerRequiredException>()
            .WithMessage($"User {userId} is not a manager in project {projectId}");
    }

    [Fact]
    public async Task CloseTaskAsyncThrowsProjectArchivedException()
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
            Archived = true,
            Members = []
        };

        this.projectRepositoryMock
            .Setup(projectRepository => projectRepository.FindOneAsync(projectId, cancellationToken))
            .ReturnsAsync(project);

        await Invoking(() => this.taskCloseUseCase.CloseTaskAsync(actor, taskId, cancellationToken))
            .Should().ThrowAsync<ProjectArchivedException>()
            .WithMessage($"Project {projectId} is archived");
    }

    [Fact]
    public async Task CloseTaskAsyncThrowsProjectNotFoundException()
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

        this.projectRepositoryMock
            .Setup(projectRepository => projectRepository.FindOneAsync(projectId, cancellationToken))
            .ReturnsAsync((Project?)null);

        await Invoking(() => this.taskCloseUseCase.CloseTaskAsync(actor, taskId, cancellationToken))
            .Should().ThrowAsync<ProjectNotFoundException>()
            .WithMessage($"Could not find project {projectId}");
    }

    [Fact]
    public async Task CloseTaskAsyncThrowsTaskNotFoundException()
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

        await Invoking(() => this.taskCloseUseCase.CloseTaskAsync(actor, taskId, cancellationToken))
            .Should().ThrowAsync<TaskNotFoundException>()
            .WithMessage($"Could not find task {taskId}");
    }
}
