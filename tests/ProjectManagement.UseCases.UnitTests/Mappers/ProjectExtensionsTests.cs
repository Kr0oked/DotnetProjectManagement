namespace DotnetProjectManagement.ProjectManagement.UseCases.UnitTests.Mappers;

using Domain.Entities;
using FluentAssertions;
using UseCases.Mappers;
using Xunit;

public class ProjectExtensionsTests
{
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
