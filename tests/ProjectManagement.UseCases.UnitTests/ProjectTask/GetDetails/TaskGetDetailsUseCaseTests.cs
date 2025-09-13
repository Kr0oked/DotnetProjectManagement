namespace DotnetProjectManagement.ProjectManagement.UseCases.UnitTests.ProjectTask.GetDetails;

using Abstractions;
using Domain.Entities;
using Exceptions;
using FluentAssertions;
using Moq;
using UseCases.DTOs;
using UseCases.ProjectTask.GetDetails;
using Xunit;
using static FluentAssertions.FluentActions;

public class TaskGetDetailsUseCaseTests
{
    private readonly TaskGetDetailsUseCase taskGetDetailsUseCase;
    private readonly Mock<ITaskRepository> taskRepositoryMock = new();
    private readonly Mock<IProjectRepository> projectRepositoryMock = new();

    public TaskGetDetailsUseCaseTests() =>
        this.taskGetDetailsUseCase = new TaskGetDetailsUseCase(
            this.taskRepositoryMock.Object,
            this.projectRepositoryMock.Object);

    [Fact]
    public async Task GetTaskDetailsAsyncAsMember()
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

        var taskDto = await this.taskGetDetailsUseCase.GetTaskDetailsAsync(actor, taskId, cancellationToken);

        taskDto.Id.Should().Be(taskId);
        taskDto.DisplayName.Should().Be("DisplayName");
        taskDto.Description.Should().Be("Description");
        taskDto.Open.Should().BeFalse();
        taskDto.Assignees.Should().Equal(userId);
    }

    [Fact]
    public async Task GetTaskDetailsAsyncAsAdministrator()
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

        var taskDto = await this.taskGetDetailsUseCase.GetTaskDetailsAsync(actor, taskId, cancellationToken);

        taskDto.Id.Should().Be(taskId);
        taskDto.DisplayName.Should().Be("DisplayName");
        taskDto.Description.Should().Be("Description");
        taskDto.Open.Should().BeFalse();
        taskDto.Assignees.Should().Equal(userId);
    }

    [Fact]
    public async Task GetTaskDetailsAsyncThrowsMemberRequiredException()
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

        await Invoking(() => this.taskGetDetailsUseCase.GetTaskDetailsAsync(actor, taskId, cancellationToken))
            .Should().ThrowAsync<ProjectMemberException>()
            .WithMessage($"User {userId} is not a member of project {projectId}");
    }

    [Fact]
    public async Task GetTaskDetailsAsyncThrowsProjectNotFoundException()
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

        await Invoking(() => this.taskGetDetailsUseCase.GetTaskDetailsAsync(actor, taskId, cancellationToken))
            .Should().ThrowAsync<ProjectNotFoundException>()
            .WithMessage($"Could not find project {projectId}");
    }

    [Fact]
    public async Task GetTaskDetailsAsyncThrowsTaskNotFoundException()
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

        await Invoking(() => this.taskGetDetailsUseCase.GetTaskDetailsAsync(actor, taskId, cancellationToken))
            .Should().ThrowAsync<TaskNotFoundException>()
            .WithMessage($"Could not find task {taskId}");
    }
}
