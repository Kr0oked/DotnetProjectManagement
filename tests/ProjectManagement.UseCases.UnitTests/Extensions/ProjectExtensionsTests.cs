namespace DotnetProjectManagement.ProjectManagement.UseCases.UnitTests.Extensions;

using Domain.Entities;
using Exceptions;
using FluentAssertions;
using UseCases.Extensions;
using UseCases.Project.Restore;
using Xunit;
using static FluentAssertions.FluentActions;

public class ProjectExtensionsTests
{
    [Fact]
    public void VerifyProjectIsArchivedDoesNotThrowWhenProjectIsArchived()
    {
        var projectId = new Guid("5cc2a903-7672-4f78-b360-b573b6469ee3");
        var project = new Project
        {
            Id = projectId,
            DisplayName = "DisplayName",
            Archived = true,
            Members = []
        };

        Invoking(project.VerifyProjectIsArchived)
            .Should().NotThrow();
    }

    [Fact]
    public void VerifyProjectIsArchivedThrowsProjectNotArchivedExceptionWhenProjectIsNotArchived()
    {
        var projectId = new Guid("5cc2a903-7672-4f78-b360-b573b6469ee3");
        var project = new Project
        {
            Id = projectId,
            DisplayName = "DisplayName",
            Archived = false,
            Members = []
        };

        Invoking(project.VerifyProjectIsArchived)
            .Should().Throw<ProjectNotArchivedException>()
            .WithMessage($"Project {projectId} is not archived");
    }

    [Fact]
    public void VerifyProjectIsNotArchivedDoesNotThrowWhenProjectIsNotArchived()
    {
        var projectId = new Guid("5cc2a903-7672-4f78-b360-b573b6469ee3");
        var project = new Project
        {
            Id = projectId,
            DisplayName = "DisplayName",
            Archived = false,
            Members = []
        };

        Invoking(project.VerifyProjectIsNotArchived)
            .Should().NotThrow();
    }

    [Fact]
    public void VerifyProjectIsNotArchivedThrowsProjectArchivedExceptionWhenProjectIsArchived()
    {
        var projectId = new Guid("5cc2a903-7672-4f78-b360-b573b6469ee3");
        var project = new Project
        {
            Id = projectId,
            DisplayName = "DisplayName",
            Archived = true,
            Members = []
        };

        Invoking(project.VerifyProjectIsNotArchived)
            .Should().Throw<ProjectArchivedException>()
            .WithMessage($"Project {projectId} is archived");
    }

    [Fact]
    public void ToDto()
    {
        var projectId = new Guid("5cc2a903-7672-4f78-b360-b573b6469ee3");
        var userId = new Guid("fd9f45e1-48f0-42ae-a390-2f4d1653451f");
        var project = new Project
        {
            Id = projectId,
            DisplayName = "DisplayName",
            Archived = true,
            Members = new Dictionary<Guid, ProjectMemberRole> { { userId, ProjectMemberRole.Manager } }
        };

        var dto = project.ToDto();

        dto.Id.Should().Be(projectId);
        dto.DisplayName.Should().Be("DisplayName");
        dto.Archived.Should().BeTrue();
        dto.Members.Should().Equal(new Dictionary<Guid, ProjectMemberRole> { { userId, ProjectMemberRole.Manager } });
    }
}
