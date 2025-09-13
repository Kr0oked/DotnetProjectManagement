namespace DotnetProjectManagement.ProjectManagement.UseCases.UnitTests.ProjectTask.List;

using Abstractions;
using Domain.Entities;
using Exceptions;
using FluentAssertions;
using Moq;
using UseCases.DTOs;
using UseCases.ProjectTask.ListForProject;
using Xunit;
using static FluentAssertions.FluentActions;

public class TaskListForProjectUseCaseTests
{
    private readonly TaskListForProjectUseCase taskListForProjectUseCase;
    private readonly Mock<ITaskRepository> taskRepositoryMock = new();
    private readonly Mock<IProjectRepository> projectRepositoryMock = new();

    public TaskListForProjectUseCaseTests() =>
        this.taskListForProjectUseCase = new TaskListForProjectUseCase(
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
        var pageRequest = new PageRequest(0, 10);
        var cancellationToken = CancellationToken.None;

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
            .Setup(taskRepository => taskRepository.FindAllByProjectIdAsync(projectId, pageRequest, cancellationToken))
            .ReturnsAsync(new Page<ProjectTask>([task], pageRequest, 1));

        var page = await this.taskListForProjectUseCase
            .ListTasksForProjectAsync(actor, projectId, pageRequest, cancellationToken);

        page.Size.Should().Be(10);
        page.TotalElements.Should().Be(1);
        page.TotalPages.Should().Be(1);
        page.Number.Should().Be(0);
        page.Content.Should().SatisfyRespectively(taskDto =>
        {
            taskDto.Id.Should().Be(taskId);
            taskDto.DisplayName.Should().Be("DisplayName");
            taskDto.Description.Should().Be("Description");
            taskDto.Open.Should().BeFalse();
            taskDto.Assignees.Should().Equal(userId);
        });
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
        var pageRequest = new PageRequest(0, 10);
        var cancellationToken = CancellationToken.None;

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
            .Setup(taskRepository => taskRepository.FindAllByProjectIdAsync(projectId, pageRequest, cancellationToken))
            .ReturnsAsync(new Page<ProjectTask>([task], pageRequest, 1));

        var page = await this.taskListForProjectUseCase
            .ListTasksForProjectAsync(actor, projectId, pageRequest, cancellationToken);

        page.Size.Should().Be(10);
        page.TotalElements.Should().Be(1);
        page.TotalPages.Should().Be(1);
        page.Number.Should().Be(0);
        page.Content.Should().SatisfyRespectively(taskDto =>
        {
            taskDto.Id.Should().Be(taskId);
            taskDto.DisplayName.Should().Be("DisplayName");
            taskDto.Description.Should().Be("Description");
            taskDto.Open.Should().BeFalse();
            taskDto.Assignees.Should().Equal(userId);
        });
    }

    [Fact]
    public async Task GetTaskDetailsAsyncThrowsMemberRequiredException()
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
        var pageRequest = new PageRequest(0, 10);
        var cancellationToken = CancellationToken.None;

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

        await Invoking(() =>
                this.taskListForProjectUseCase.ListTasksForProjectAsync(actor, projectId, pageRequest,
                    cancellationToken))
            .Should().ThrowAsync<ProjectMemberException>()
            .WithMessage($"User {userId} is not a member of project {projectId}");
    }
}
