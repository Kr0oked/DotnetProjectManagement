namespace DotnetProjectManagement.ProjectManagement.UseCases.UnitTests.Project.GetDetails;

using Abstractions;
using Domain.Entities;
using Exceptions;
using FluentAssertions;
using Moq;
using UseCases.DTOs;
using UseCases.Project.GetDetails;
using Xunit;
using static FluentAssertions.FluentActions;

public class ProjectGetDetailsUseCaseTests
{
    private readonly ProjectGetDetailsUseCase projectGetDetailsUseCase;
    private readonly Mock<IProjectRepository> projectRepositoryMock = new();

    public ProjectGetDetailsUseCaseTests() =>
        this.projectGetDetailsUseCase = new ProjectGetDetailsUseCase(this.projectRepositoryMock.Object);

    [Fact]
    public async Task GetProjectDetailsAsyncAsMember()
    {
        var projectId = new Guid("00aaba25-ffad-4564-bffc-57a3b85fc171");
        var userId = new Guid("fd9f45e1-48f0-42ae-a390-2f4d1653451f");
        var actor = new Actor
        {
            UserId = userId,
            IsAdministrator = false
        };
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

        var projectDto =
            await this.projectGetDetailsUseCase.GetProjectDetailsAsync(actor, projectId, cancellationToken);

        projectDto.Id.Should().Be(projectId);
        projectDto.DisplayName.Should().Be("DisplayName");
        projectDto.Archived.Should().Be(true);
        projectDto.Members.Should().Equal(new Dictionary<Guid, ProjectMemberRole>
        {
            { userId, ProjectMemberRole.Guest }
        });
    }

    [Fact]
    public async Task GetProjectDetailsAsyncAsAdministrator()
    {
        var projectId = new Guid("00aaba25-ffad-4564-bffc-57a3b85fc171");
        var actor = new Actor
        {
            UserId = new Guid("fd9f45e1-48f0-42ae-a390-2f4d1653451f"),
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

        var projectDto =
            await this.projectGetDetailsUseCase.GetProjectDetailsAsync(actor, projectId, cancellationToken);

        projectDto.Id.Should().Be(projectId);
        projectDto.DisplayName.Should().Be("DisplayName");
        projectDto.Archived.Should().Be(true);
        projectDto.Members.Should().BeEmpty();
    }

    [Fact]
    public async Task GetProjectDetailsAsyncThrowsMemberRequiredException()
    {
        var projectId = new Guid("00aaba25-ffad-4564-bffc-57a3b85fc171");
        var userId = new Guid("fd9f45e1-48f0-42ae-a390-2f4d1653451f");
        var actor = new Actor
        {
            UserId = userId,
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

        await Invoking(() => this.projectGetDetailsUseCase.GetProjectDetailsAsync(actor, projectId, cancellationToken))
            .Should().ThrowAsync<ProjectMemberException>()
            .WithMessage($"User {userId} is not a member of project {projectId}");
    }

    [Fact]
    public async Task GetProjectDetailsAsyncThrowsProjectNotFoundException()
    {
        var projectId = new Guid("00aaba25-ffad-4564-bffc-57a3b85fc171");
        var actor = new Actor
        {
            UserId = new Guid("fd9f45e1-48f0-42ae-a390-2f4d1653451f"),
            IsAdministrator = true
        };
        var cancellationToken = CancellationToken.None;

        this.projectRepositoryMock
            .Setup(projectRepository => projectRepository.FindOneAsync(projectId, cancellationToken))
            .ReturnsAsync((Project?)null);

        await Invoking(() => this.projectGetDetailsUseCase.GetProjectDetailsAsync(actor, projectId, cancellationToken))
            .Should().ThrowAsync<ProjectNotFoundException>()
            .WithMessage($"Could not find project {projectId}");
    }
}
