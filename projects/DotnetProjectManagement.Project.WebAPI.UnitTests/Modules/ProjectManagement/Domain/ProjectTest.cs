namespace DotnetProjectManagement.Project.WebAPI.UnitTests.Modules.ProjectManagement.Domain;

using FluentAssertions;
using JetBrains.Annotations;
using Moq;
using Project = WebAPI.Modules.ProjectManagement.Domain.Project;
using WebAPI.Modules.ProjectManagement.Domain;
using Xunit;

[TestSubject(typeof(Project))]
public class ProjectTest
{
    private readonly Mock<IProjectRepository> projectRepositoryMock = new();

    [Fact]
    public void ProjectProperties()
    {
        var project = new Project(this.projectRepositoryMock.Object, new ProjectData("the-id", "the-displayName"));

        project.Id.Should().Be("the-id");
        project.DisplayName.Should().Be("the-displayName");
    }

    [Fact]
    public void UpdateDisplayName()
    {
        var project = new Project(this.projectRepositoryMock.Object, new ProjectData("the-id", "the-displayName"));

        project.UpdateDisplayName("new-displayName");

        project.DisplayName.Should().Be("new-displayName");

        this.projectRepositoryMock.Verify(projectRepository =>
            projectRepository.UpdateProjectDisplayName("the-id", "new-displayName"));
        this.projectRepositoryMock.VerifyNoOtherCalls();
    }

    [Fact]
    public void GetMembers()
    {
        var project = new Project(this.projectRepositoryMock.Object, new ProjectData("the-id", "the-displayName"));

        var guest = new User(new UserData("guest", false));
        var worker = new User(new UserData("worker", false));
        var manager = new User(new UserData("manager", false));

        var members = new Dictionary<User, ProjectMemberRole>
        {
            [guest] = ProjectMemberRole.Guest,
            [worker] = ProjectMemberRole.Worker,
            [manager] = ProjectMemberRole.Manager
        };

        this.projectRepositoryMock
            .Setup(projectRepository => projectRepository.GetMembers("the-id"))
            .Returns(members);

        project.Members.Should().Equal(members);

        this.projectRepositoryMock.Verify(projectRepository => projectRepository.GetMembers("the-id"));
        this.projectRepositoryMock.VerifyNoOtherCalls();
    }

    [Fact]
    public void UpdateMembers()
    {
        var project = new Project(this.projectRepositoryMock.Object, new ProjectData("the-id", "the-displayName"));

        var guest = new User(new UserData("guest", false));
        var worker = new User(new UserData("worker", false));
        var manager = new User(new UserData("manager", false));

        var members = new Dictionary<User, ProjectMemberRole>
        {
            [guest] = ProjectMemberRole.Guest,
            [worker] = ProjectMemberRole.Worker,
            [manager] = ProjectMemberRole.Manager
        };

        project.UpdateMembers(members);

        project.Members.Should().Equal(members);
    }
}
