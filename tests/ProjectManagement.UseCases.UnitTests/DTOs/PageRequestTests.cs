namespace DotnetProjectManagement.ProjectManagement.UseCases.UnitTests.DTOs;

using FluentAssertions;
using UseCases.DTOs;
using Xunit;
using static FluentAssertions.FluentActions;

public class PageRequestTests
{
    [Fact]
    public void FirstPage()
    {
        var pageRequest = new PageRequest(0, 10);
        pageRequest.Number.Should().Be(0);
        pageRequest.Size.Should().Be(10);
        pageRequest.Offset.Should().Be(0);
    }

    [Fact]
    public void SecondPage()
    {
        var pageRequest = new PageRequest(1, 10);
        pageRequest.Number.Should().Be(1);
        pageRequest.Size.Should().Be(10);
        pageRequest.Offset.Should().Be(10);
    }

    [Fact]
    public void ThirdPage()
    {
        var pageRequest = new PageRequest(2, 10);
        pageRequest.Number.Should().Be(2);
        pageRequest.Size.Should().Be(10);
        pageRequest.Offset.Should().Be(20);
    }

    [Fact]
    public void NumberMustBePositiveOrZero() =>
        Invoking(() => new PageRequest(-1, 10)).Should().Throw<ArgumentOutOfRangeException>();

    [Fact]
    public void SizeMustBePositive() =>
        Invoking(() => new PageRequest(0, 0)).Should().Throw<ArgumentOutOfRangeException>();
}
