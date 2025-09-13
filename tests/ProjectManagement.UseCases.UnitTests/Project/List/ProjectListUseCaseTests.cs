namespace DotnetProjectManagement.ProjectManagement.UseCases.UnitTests.Project.List;

using Abstractions;
using Domain.Entities;
using FluentAssertions;
using Moq;
using UseCases.DTOs;
using UseCases.Project.List;
using Xunit;

public class ProjectListUseCaseTests
{
    private readonly ProjectListUseCase projectListUseCase;
    private readonly Mock<IProjectRepository> projectRepositoryMock = new();

    public ProjectListUseCaseTests() =>
        this.projectListUseCase = new ProjectListUseCase(this.projectRepositoryMock.Object);

    [Fact]
    public async Task ListProjectsAsyncAsNormalUser()
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
            Members = new Dictionary<Guid, ProjectMemberRole> { { userId, ProjectMemberRole.Guest } }
        };

        this.projectRepositoryMock
            .Setup(userRepository =>
                userRepository.FindByNotArchivedAndMembershipAsync(userId, pageRequest, cancellationToken))
            .ReturnsAsync(new Page<Project>([project], pageRequest, 1));

        var page = await this.projectListUseCase.ListProjectsAsync(actor, pageRequest, cancellationToken);

        page.Size.Should().Be(10);
        page.TotalElements.Should().Be(1);
        page.TotalPages.Should().Be(1);
        page.Number.Should().Be(0);
        page.Content.Should().SatisfyRespectively(projectDto =>
        {
            projectDto.Id.Should().Be(projectId);
            projectDto.DisplayName.Should().Be("DisplayName");
            projectDto.Archived.Should().Be(true);
            projectDto.Members.Should().Equal(new Dictionary<Guid, ProjectMemberRole>
            {
                { userId, ProjectMemberRole.Guest }
            });
        });
    }

    [Fact]
    public async Task ListProjectsAsyncAsAdministrator()
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
            .Setup(userRepository => userRepository.FindAllAsync(pageRequest, cancellationToken))
            .ReturnsAsync(new Page<Project>([project], pageRequest, 1));

        var page = await this.projectListUseCase.ListProjectsAsync(actor, pageRequest, cancellationToken);

        page.Size.Should().Be(10);
        page.TotalElements.Should().Be(1);
        page.TotalPages.Should().Be(1);
        page.Number.Should().Be(0);
        page.Content.Should().SatisfyRespectively(projectDto =>
        {
            projectDto.Id.Should().Be(projectId);
            projectDto.DisplayName.Should().Be("DisplayName");
            projectDto.Archived.Should().Be(true);
            projectDto.Members.Should().Equal(new Dictionary<Guid, ProjectMemberRole>
            {
                { userId, ProjectMemberRole.Guest }
            });
        });
    }
}
