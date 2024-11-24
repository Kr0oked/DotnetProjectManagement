namespace DotnetProjectManagement.Project.WebAPI.UnitTests.Modules.ProjectManagement.Domain;

using FluentAssertions;
using JetBrains.Annotations;
using Moq;
using WebAPI.Modules.ProjectManagement.Domain;
using Xunit;

[TestSubject(typeof(ProjectFactory))]
public class ProjectFactoryTest
{
    private readonly ProjectFactory projectFactory;
    private readonly Mock<IProjectRepository> projectRepositoryMock = new();

    public ProjectFactoryTest() => this.projectFactory = new ProjectFactory(this.projectRepositoryMock.Object);

    [Fact]
    public void CreateProject()
    {
        this.projectRepositoryMock
            .Setup(projectRepository => projectRepository.CreatProject("the-displayName"))
            .Returns(new ProjectData("the-id", "the-displayName"));

        var project = this.projectFactory.CreateProject("the-displayName");

        project.Id.Should().Be("the-id");
        project.DisplayName.Should().Be("the-displayName");
    }

    [Fact]
    public void GetProject()
    {
        this.projectRepositoryMock
            .Setup(projectRepository => projectRepository.FindProjectById("the-id"))
            .Returns(new ProjectData("the-id", "the-displayName"));

        var project = this.projectFactory.GetProject("the-id");

        project.Id.Should().Be("the-id");
        project.DisplayName.Should().Be("the-displayName");
    }

    [Fact]
    public void GetProjectWithUnknownIdThrowsException() =>
        this.projectFactory.Invoking(pf => pf.GetProject("the-id"))
            .Should().Throw<ProjectNotFoundException>()
            .WithMessage("Project with ID 'the-id' was not found.");
}
