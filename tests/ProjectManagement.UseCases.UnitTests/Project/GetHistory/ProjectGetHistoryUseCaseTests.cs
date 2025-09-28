namespace DotnetProjectManagement.ProjectManagement.UseCases.UnitTests.Project.GetHistory;

using Domain.Entities;
using Abstractions;
using Domain.Actions;
using DotnetProjectManagement.ProjectManagement.UseCases.DTOs;
using Exceptions;
using FluentAssertions;
using Moq;
using UseCases.Project.GetHistory;
using Xunit;
using static FluentAssertions.FluentActions;

public class ProjectGetHistoryUseCaseTests
{
    private readonly ProjectGetHistoryUseCase projectGetHistoryUseCase;
    private readonly Mock<IProjectRepository> projectRepositoryMock = new();

    public ProjectGetHistoryUseCaseTests() =>
        this.projectGetHistoryUseCase = new ProjectGetHistoryUseCase(this.projectRepositoryMock.Object);

    [Fact]
    public async Task GetProjectHistoryAsyncAsMember()
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

        var project = new Project
        {
            Id = projectId,
            DisplayName = "DisplayName",
            Archived = true,
            Members = new Dictionary<Guid, ProjectMemberRole> { { userId, ProjectMemberRole.Worker } }
        };

        this.projectRepositoryMock
            .Setup(projectRepository => projectRepository.FindOneAsync(projectId, cancellationToken))
            .ReturnsAsync(project);

        var historyEntry = new HistoryEntry<ProjectAction, Project>
        {
            Action = ProjectAction.Create,
            Entity = project,
            Timestamp = new DateTime(12345),
            User = new UserDto
            {
                Id = userId,
                FirstName = "FirstName",
                LastName = "LastName"
            }
        };

        this.projectRepositoryMock
            .Setup(projectRepository => projectRepository.GetHistory(projectId, cancellationToken))
            .ReturnsAsync([historyEntry]);

        var history =
            await this.projectGetHistoryUseCase.GetProjectHistoryAsync(actor, projectId, cancellationToken);

        history.Should().SatisfyRespectively(entry =>
        {
            entry.Action.Should().Be(ProjectAction.Create);
            entry.Entity.Id.Should().Be(projectId);
            entry.Entity.DisplayName.Should().Be("DisplayName");
            entry.Entity.Archived.Should().Be(true);
            entry.Entity.Members.Should().Equal(new Dictionary<Guid, ProjectMemberRole>
            {
                { userId, ProjectMemberRole.Worker }
            });
            entry.Timestamp.Should().Be(new DateTime(12345));
            entry.User.Id.Should().Be(userId);
            entry.User.FirstName.Should().Be("FirstName");
            entry.User.LastName.Should().Be("LastName");
        });
    }

    [Fact]
    public async Task GetProjectHistoryAsyncAsAdministrator()
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

        var historyEntry = new HistoryEntry<ProjectAction, Project>
        {
            Action = ProjectAction.Create,
            Entity = project,
            Timestamp = new DateTime(12345),
            User = new UserDto
            {
                Id = userId,
                FirstName = "FirstName",
                LastName = "LastName"
            }
        };

        this.projectRepositoryMock
            .Setup(projectRepository => projectRepository.GetHistory(projectId, cancellationToken))
            .ReturnsAsync([historyEntry]);

        var history =
            await this.projectGetHistoryUseCase.GetProjectHistoryAsync(actor, projectId, cancellationToken);

        history.Should().SatisfyRespectively(entry =>
        {
            entry.Action.Should().Be(ProjectAction.Create);
            entry.Entity.Id.Should().Be(projectId);
            entry.Entity.DisplayName.Should().Be("DisplayName");
            entry.Entity.Archived.Should().Be(true);
            entry.Entity.Members.Should().BeEmpty();
            entry.Timestamp.Should().Be(new DateTime(12345));
            entry.User.Id.Should().Be(userId);
            entry.User.FirstName.Should().Be("FirstName");
            entry.User.LastName.Should().Be("LastName");
        });
    }

    [Fact]
    public async Task GetProjectHistoryAsyncThrowsMemberRequiredException()
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

        await Invoking(() => this.projectGetHistoryUseCase.GetProjectHistoryAsync(actor, projectId, cancellationToken))
            .Should().ThrowAsync<ProjectMemberException>()
            .WithMessage($"User {userId} is not a member of project {projectId}");
    }

    [Fact]
    public async Task GetProjectHistoryAsyncThrowsProjectNotFoundException()
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

        this.projectRepositoryMock
            .Setup(projectRepository => projectRepository.FindOneAsync(projectId, cancellationToken))
            .ReturnsAsync((Project?)null);

        await Invoking(() => this.projectGetHistoryUseCase.GetProjectHistoryAsync(actor, projectId, cancellationToken))
            .Should().ThrowAsync<ProjectNotFoundException>()
            .WithMessage($"Could not find project {projectId}");
    }
}
