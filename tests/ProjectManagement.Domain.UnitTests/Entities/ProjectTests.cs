namespace DotnetProjectManagement.ProjectManagement.Domain.UnitTests.Entities;

using Domain.Entities;
using FluentAssertions;
using Xunit;

public class ProjectTests
{
    [Fact]
    public void ToStringShouldContainFields()
    {
        var project = new Project
        {
            Id = new Guid("5cc2a903-7672-4f78-b360-b573b6469ee3"),
            DisplayName = "DisplayName",
            Archived = true,
            Members = new Dictionary<Guid, ProjectMemberRole>
            {
                { new Guid("fd9f45e1-48f0-42ae-a390-2f4d1653451f"), ProjectMemberRole.Guest },
                { new Guid("4ccdd441-fd29-42f5-a588-446fb9c94640"), ProjectMemberRole.Worker }
            }
        };

        var result = project.ToString();

        result.Should().Be(
            "Project { " +
            "Id = 5cc2a903-7672-4f78-b360-b573b6469ee3, " +
            "DisplayName = DisplayName, " +
            "Archived = True, " +
            "Members = { " +
            "[4ccdd441-fd29-42f5-a588-446fb9c94640, Worker], " +
            "[fd9f45e1-48f0-42ae-a390-2f4d1653451f, Guest] " +
            "} " +
            "}");
    }

    [Fact]
    public void GetRoleOfUserShouldReturnGuest()
    {
        var project = new Project
        {
            Id = new Guid("5cc2a903-7672-4f78-b360-b573b6469ee3"),
            DisplayName = "DisplayName",
            Archived = true,
            Members = new Dictionary<Guid, ProjectMemberRole>
            {
                { new Guid("fd9f45e1-48f0-42ae-a390-2f4d1653451f"), ProjectMemberRole.Guest }
            }
        };

        var role = project.GetRoleOfUser(new Guid("fd9f45e1-48f0-42ae-a390-2f4d1653451f"));

        role.Should().Be(ProjectMemberRole.Guest);
    }

    [Fact]
    public void GetRoleOfUserShouldReturnWorker()
    {
        var project = new Project
        {
            Id = new Guid("5cc2a903-7672-4f78-b360-b573b6469ee3"),
            DisplayName = "DisplayName",
            Archived = true,
            Members = new Dictionary<Guid, ProjectMemberRole>
            {
                { new Guid("fd9f45e1-48f0-42ae-a390-2f4d1653451f"), ProjectMemberRole.Worker }
            }
        };

        var role = project.GetRoleOfUser(new Guid("fd9f45e1-48f0-42ae-a390-2f4d1653451f"));

        role.Should().Be(ProjectMemberRole.Worker);
    }

    [Fact]
    public void GetRoleOfUserShouldReturnManager()
    {
        var project = new Project
        {
            Id = new Guid("5cc2a903-7672-4f78-b360-b573b6469ee3"),
            DisplayName = "DisplayName",
            Archived = true,
            Members = new Dictionary<Guid, ProjectMemberRole>
            {
                { new Guid("fd9f45e1-48f0-42ae-a390-2f4d1653451f"), ProjectMemberRole.Manager }
            }
        };

        var role = project.GetRoleOfUser(new Guid("fd9f45e1-48f0-42ae-a390-2f4d1653451f"));

        role.Should().Be(ProjectMemberRole.Manager);
    }

    [Fact]
    public void GetRoleOfUserShouldReturnNull()
    {
        var project = new Project
        {
            Id = new Guid("5cc2a903-7672-4f78-b360-b573b6469ee3"),
            DisplayName = "DisplayName",
            Archived = true,
            Members = []
        };

        var role = project.GetRoleOfUser(new Guid("fd9f45e1-48f0-42ae-a390-2f4d1653451f"));

        role.Should().BeNull();
    }
}
