namespace DotnetProjectManagement.ProjectManagement.UseCases.UnitTests.DTOs;

using System.Globalization;
using Domain.Actions;
using FluentAssertions;
using UseCases.DTOs;
using Xunit;

public class HistoryEntryTests
{
    [Fact]
    public void Map()
    {
        var timestamp = DateTime.Parse("2011-03-21 13:26", CultureInfo.InvariantCulture);

        var user = new UserDto()
        {
            Id = new Guid("fd9f45e1-48f0-42ae-a390-2f4d1653451f"),
            FirstName = "FirstName",
            LastName = "LastName"
        };

        var historyEntry = new HistoryEntry<ProjectAction, int>
        {
            Action = ProjectAction.Create,
            Entity = 1,
            Timestamp = DateTime.Parse("2011-03-21 13:26", CultureInfo.InvariantCulture),
            User = user
        };

        var mappedHistoryEntry = historyEntry.Map(s => s.ToString(CultureInfo.InvariantCulture));

        mappedHistoryEntry.Action.Should().Be(ProjectAction.Create);
        mappedHistoryEntry.Entity.Should().Be("1");
        mappedHistoryEntry.Timestamp.Should().Be(timestamp);
        mappedHistoryEntry.User.Should().Be(user);
    }
}
