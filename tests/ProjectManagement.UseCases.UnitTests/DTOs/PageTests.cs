namespace DotnetProjectManagement.ProjectManagement.UseCases.UnitTests.DTOs;

using System.Globalization;
using FluentAssertions;
using UseCases.DTOs;
using Xunit;

public class PageTests
{
    [Fact]
    public void FirstPage()
    {
        var pageRequest = new PageRequest(0, 3);
        var page = new Page<string>(["a", "b", "c"], pageRequest, 4);

        page.Size.Should().Be(3);
        page.TotalElements.Should().Be(4);
        page.TotalPages.Should().Be(2);
        page.Number.Should().Be(0);
        page.Content.Should().Equal("a", "b", "c");
    }

    [Fact]
    public void SecondPage()
    {
        var pageRequest = new PageRequest(1, 3);
        var page = new Page<string>(["d"], pageRequest, 4);

        page.Size.Should().Be(3);
        page.TotalElements.Should().Be(4);
        page.TotalPages.Should().Be(2);
        page.Number.Should().Be(1);
        page.Content.Should().Equal("d");
    }

    [Fact]
    public void Map()
    {
        var pageRequest = new PageRequest(0, 3);
        var page = new Page<int>([1, 2, 3], pageRequest, 4);
        var mappedPage = page.Map<string>(s => s.ToString(CultureInfo.InvariantCulture));

        mappedPage.Size.Should().Be(3);
        mappedPage.TotalElements.Should().Be(4);
        mappedPage.TotalPages.Should().Be(2);
        mappedPage.Number.Should().Be(0);
        mappedPage.Content.Should().Equal("1", "2", "3");
    }
}
