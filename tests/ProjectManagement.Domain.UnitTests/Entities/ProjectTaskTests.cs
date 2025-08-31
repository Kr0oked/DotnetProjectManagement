namespace DotnetProjectManagement.ProjectManagement.Domain.UnitTests.Entities;

using Domain.Entities;
using FluentAssertions;
using Xunit;

public class ProjectTaskTests
{
    [Fact]
    public void ToStringShouldContainFields()
    {
        var task = new ProjectTask
        {
            Id = new Guid("5cc2a903-7672-4f78-b360-b573b6469ee3"),
            DisplayName = "DisplayName",
            Description = "Description",
            Open = true,
            Assignees =
            [
                new Guid("fd9f45e1-48f0-42ae-a390-2f4d1653451f"),
                new Guid("4ccdd441-fd29-42f5-a588-446fb9c94640")
            ],
            ProjectId = new Guid("2fe6acb3-5d7f-4a62-bded-1a3a2a96e142")
        };

        var result = task.ToString();

        result.Should().Be(
            "Task { " +
            "Id = 5cc2a903-7672-4f78-b360-b573b6469ee3, " +
            "DisplayName = DisplayName, " +
            "Description = Description, " +
            "Open = True, " +
            "Assignees = [ " +
            "4ccdd441-fd29-42f5-a588-446fb9c94640, " +
            "fd9f45e1-48f0-42ae-a390-2f4d1653451f " +
            "], " +
            "ProjectId = 2fe6acb3-5d7f-4a62-bded-1a3a2a96e142 " +
            "}");
    }
}
