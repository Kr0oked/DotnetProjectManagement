namespace DotnetProjectManagement.ProjectManagement.UseCases.UnitTests.Extensions;

using Domain.Entities;
using FluentAssertions;
using UseCases.Extensions;
using Xunit;

public class TaskExtensionsTests
{
    [Fact]
    public void ToDto()
    {
        var taskId = new Guid("5cc2a903-7672-4f78-b360-b573b6469ee3");
        var assigneeA = new Guid("fd9f45e1-48f0-42ae-a390-2f4d1653451f");
        var assigneeB = new Guid("6d002590-ba39-479b-a18c-8f75e244ff00");
        var task = new ProjectTask
        {
            Id = taskId,
            DisplayName = "DisplayName",
            Description = "Description",
            Open = true,
            Assignees = [assigneeA, assigneeB],
            ProjectId = new Guid("c850a9ba-23cd-4557-8b83-d96ffb240f34")
        };

        var dto = task.ToDto();

        dto.Id.Should().Be(taskId);
        dto.DisplayName.Should().Be("DisplayName");
        dto.Description.Should().Be("Description");
        dto.Open.Should().BeTrue();
        dto.Assignees.Should().Equal(assigneeA, assigneeB);
    }
}
