namespace DotnetProjectManagement.ProjectManagement.UseCases.UnitTests.ProjectTask.GetHistory;

using Domain.Actions;
using Domain.Entities;
using Abstractions;
using DotnetProjectManagement.ProjectManagement.UseCases.DTOs;
using Exceptions;
using FluentAssertions;
using Moq;
using UseCases.ProjectTask.GetHistory;
using Xunit;
using static FluentAssertions.FluentActions;

public class TaskGetHistoryUseCaseTests
{
    private readonly TaskGetHistoryUseCase taskGetHistoryUseCase;
    private readonly Mock<ITaskRepository> taskRepositoryMock = new();
    private readonly Mock<IProjectRepository> projectRepositoryMock = new();

    public TaskGetHistoryUseCaseTests() =>
        this.taskGetHistoryUseCase = new TaskGetHistoryUseCase(
            this.taskRepositoryMock.Object,
            this.projectRepositoryMock.Object);

    [Fact]
    public async Task GetProjectHistoryAsyncAsMember()
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
            DisplayName = "DisplayName",
            Archived = true,
            Members = new Dictionary<Guid, ProjectMemberRole> { { userId, ProjectMemberRole.Guest } }
        };

        this.projectRepositoryMock
            .Setup(projectRepository => projectRepository.FindOneAsync(projectId, cancellationToken))
            .ReturnsAsync(project);

        var historyEntry = new HistoryEntry<TaskAction, ProjectTask>
        {
            Action = TaskAction.Create,
            Entity = task,
            Timestamp = new DateTime(12345),
            User = new UserDto
            {
                Id = userId,
                FirstName = "FirstName",
                LastName = "LastName"
            }
        };

        this.taskRepositoryMock
            .Setup(taskRepository => taskRepository.GetHistory(taskId, cancellationToken))
            .ReturnsAsync([historyEntry]);

        var history = await this.taskGetHistoryUseCase.GetTaskHistoryAsync(actor, taskId, cancellationToken);

        history.Should().SatisfyRespectively(entry =>
        {
            entry.Action.Should().Be(TaskAction.Create);
            entry.Entity.Id.Should().Be(taskId);
            entry.Entity.DisplayName.Should().Be("DisplayName");
            entry.Entity.Description.Should().Be("Description");
            entry.Entity.Open.Should().BeFalse();
            entry.Entity.Assignees.Should().Equal(userId);
            entry.Timestamp.Should().Be(new DateTime(12345));
            entry.User.Id.Should().Be(userId);
            entry.User.FirstName.Should().Be("FirstName");
            entry.User.LastName.Should().Be("LastName");
        });
    }

    [Fact]
    public async Task GetProjectHistoryAsyncAsAdministrator()
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
            DisplayName = "DisplayName",
            Archived = true,
            Members = []
        };

        this.projectRepositoryMock
            .Setup(projectRepository => projectRepository.FindOneAsync(projectId, cancellationToken))
            .ReturnsAsync(project);

        var historyEntry = new HistoryEntry<TaskAction, ProjectTask>
        {
            Action = TaskAction.Create,
            Entity = task,
            Timestamp = new DateTime(12345),
            User = new UserDto
            {
                Id = userId,
                FirstName = "FirstName",
                LastName = "LastName"
            }
        };

        this.taskRepositoryMock
            .Setup(taskRepository => taskRepository.GetHistory(taskId, cancellationToken))
            .ReturnsAsync([historyEntry]);

        var history = await this.taskGetHistoryUseCase.GetTaskHistoryAsync(actor, taskId, cancellationToken);

        history.Should().SatisfyRespectively(entry =>
        {
            entry.Action.Should().Be(TaskAction.Create);
            entry.Entity.Id.Should().Be(taskId);
            entry.Entity.DisplayName.Should().Be("DisplayName");
            entry.Entity.Description.Should().Be("Description");
            entry.Entity.Open.Should().BeFalse();
            entry.Entity.Assignees.Should().Equal(userId);
            entry.Timestamp.Should().Be(new DateTime(12345));
            entry.User.Id.Should().Be(userId);
            entry.User.FirstName.Should().Be("FirstName");
            entry.User.LastName.Should().Be("LastName");
        });
    }

    [Fact]
    public async Task GetTaskHistoryAsyncThrowsMemberRequiredException()
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
            DisplayName = "DisplayName",
            Archived = true,
            Members = []
        };

        this.projectRepositoryMock
            .Setup(projectRepository => projectRepository.FindOneAsync(projectId, cancellationToken))
            .ReturnsAsync(project);

        await Invoking(() => this.taskGetHistoryUseCase.GetTaskHistoryAsync(actor, taskId, cancellationToken))
            .Should().ThrowAsync<ProjectMemberException>()
            .WithMessage($"User {userId} is not a member of project {projectId}");
    }

    [Fact]
    public async Task GetTaskHistoryAsyncThrowsProjectNotFoundException()
    {
        var taskId = new Guid("1ce8aedc-04ed-410b-821d-5556c3a18ccc");
        var projectId = new Guid("00aaba25-ffad-4564-bffc-57a3b85fc171");
        var actor = new Actor
        {
            UserId = new Guid("fd9f45e1-48f0-42ae-a390-2f4d1653451f"),
            FirstName = "FirstName",
            LastName = "LastName",
            IsAdministrator = false
        };
        var cancellationToken = CancellationToken.None;

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

        await Invoking(() => this.taskGetHistoryUseCase.GetTaskHistoryAsync(actor, taskId, cancellationToken))
            .Should().ThrowAsync<ProjectNotFoundException>()
            .WithMessage($"Could not find project {projectId}");
    }

    [Fact]
    public async Task GetTaskHistoryAsyncThrowsTaskNotFoundException()
    {
        var taskId = new Guid("1ce8aedc-04ed-410b-821d-5556c3a18ccc");
        var actor = new Actor
        {
            UserId = new Guid("fd9f45e1-48f0-42ae-a390-2f4d1653451f"),
            FirstName = "FirstName",
            LastName = "LastName",
            IsAdministrator = false
        };
        var cancellationToken = CancellationToken.None;

        this.taskRepositoryMock
            .Setup(taskRepository => taskRepository.FindOneAsync(taskId, cancellationToken))
            .ReturnsAsync((ProjectTask?)null);

        await Invoking(() => this.taskGetHistoryUseCase.GetTaskHistoryAsync(actor, taskId, cancellationToken))
            .Should().ThrowAsync<TaskNotFoundException>()
            .WithMessage($"Could not find task {taskId}");
    }
}
