namespace DotnetProjectManagement.ProjectManagement.IntegrationTests.Tests;

using System.Collections.Immutable;
using System.Net;
using Domain.Entities;
using FluentAssertions;
using Web.Models;
using Xunit;
using Xunit.Abstractions;
using static FluentAssertions.FluentActions;

public class ProjectApiTests(TestWebApplicationFactory<Program> testWebApplicationFactory, ITestOutputHelper output)
    : IntegrationTest(testWebApplicationFactory, output)
{
    [Fact]
    public async Task ListProjectsAsAdministratorProvidesAllProjects()
    {
        this.ActAsAdmin();

        var projectA = await this.ProjectClient.CreateProjectAsync(new ProjectSaveRequest
        {
            DisplayName = "DisplayNameA",
            Members = ImmutableDictionary<Guid, ProjectMemberRole>.Empty
        });
        var projectB = await this.ProjectClient.CreateProjectAsync(new ProjectSaveRequest
        {
            DisplayName = "DisplayNameB",
            Members = ImmutableDictionary<Guid, ProjectMemberRole>.Empty
        });
        var projectC = await this.ProjectClient.CreateProjectAsync(new ProjectSaveRequest
        {
            DisplayName = "DisplayNameC",
            Members = ImmutableDictionary<Guid, ProjectMemberRole>.Empty
        });

        var page1 = await this.ProjectClient.ListProjectsAsync(0, 2);

        page1.Number.Should().Be(0);
        page1.Size.Should().Be(2);
        page1.TotalElements.Should().Be(3);
        page1.TotalPages.Should().Be(2);
        page1.Content.Should().Equal(projectA, projectB);

        var page2 = await this.ProjectClient.ListProjectsAsync(1, 2);

        page2.Number.Should().Be(1);
        page2.Size.Should().Be(2);
        page2.TotalElements.Should().Be(3);
        page2.TotalPages.Should().Be(2);
        page2.Content.Should().Equal(projectC);
    }

    [Fact]
    public async Task ListProjectsAsNormalUserProvidesActiveProjectsOfUser()
    {
        this.ActAsAdmin();

        var activeUserProjectA = await this.ProjectClient.CreateProjectAsync(new ProjectSaveRequest
        {
            DisplayName = "DisplayNameActiveUserProjectA",
            Members = new Dictionary<Guid, ProjectMemberRole> { { DefaultUserGuid, ProjectMemberRole.Manager } }
                .ToImmutableDictionary()
        });

        var archivedProject = await this.ProjectClient.CreateProjectAsync(new ProjectSaveRequest
        {
            DisplayName = "DisplayNameArchivedProject",
            Members = new Dictionary<Guid, ProjectMemberRole> { { DefaultUserGuid, ProjectMemberRole.Manager } }
                .ToImmutableDictionary()
        });
        await this.ProjectClient.ArchiveProjectAsync(archivedProject.Id);

        var activeUserProjectB = await this.ProjectClient.CreateProjectAsync(new ProjectSaveRequest
        {
            DisplayName = "DisplayNameActiveUserProjectB",
            Members = new Dictionary<Guid, ProjectMemberRole> { { DefaultUserGuid, ProjectMemberRole.Guest } }
                .ToImmutableDictionary()
        });

        await this.ProjectClient.CreateProjectAsync(new ProjectSaveRequest
        {
            DisplayName = "DisplayNameActiveAdminProject",
            Members = new Dictionary<Guid, ProjectMemberRole> { { DefaultAdminGuid, ProjectMemberRole.Worker } }
                .ToImmutableDictionary()
        });

        var activeUserProjectC = await this.ProjectClient.CreateProjectAsync(new ProjectSaveRequest
        {
            DisplayName = "DisplayNameActiveUserProjectC",
            Members = new Dictionary<Guid, ProjectMemberRole> { { DefaultUserGuid, ProjectMemberRole.Worker } }
                .ToImmutableDictionary()
        });

        this.ActAsUser();

        var page1 = await this.ProjectClient.ListProjectsAsync(0, 2);

        page1.Number.Should().Be(0);
        page1.Size.Should().Be(2);
        page1.TotalElements.Should().Be(3);
        page1.TotalPages.Should().Be(2);
        page1.Content.Should().Equal(activeUserProjectA, activeUserProjectB);

        var page2 = await this.ProjectClient.ListProjectsAsync(1, 2);

        page2.Number.Should().Be(1);
        page2.Size.Should().Be(2);
        page2.TotalElements.Should().Be(3);
        page2.TotalPages.Should().Be(2);
        page2.Content.Should().Equal(activeUserProjectC);
    }

    [Fact]
    public async Task CreateProjectAsAdministrator()
    {
        this.ActAsAdmin();

        var project = await this.ProjectClient.CreateProjectAsync(new ProjectSaveRequest
        {
            DisplayName = "DisplayName",
            Members = new Dictionary<Guid, ProjectMemberRole> { { DefaultUserGuid, ProjectMemberRole.Manager } }
                .ToImmutableDictionary()
        });

        project.DisplayName.Should().Be("DisplayName");
        project.Archived.Should().BeFalse();
        project.Members.Should().SatisfyRespectively(member =>
        {
            var (memberUserId, memberRole) = member;
            memberUserId.Should().Be(DefaultUserGuid);
            memberRole.Should().Be(ProjectMemberRole.Manager);
        });
    }

    [Fact]
    public async Task CreateProjectAsNormalUserIsForbidden()
    {
        this.ActAsUser();

        await Invoking(() => this.ProjectClient.CreateProjectAsync(
                new ProjectSaveRequest
                {
                    DisplayName = "DisplayName",
                    Members = ImmutableDictionary<Guid, ProjectMemberRole>.Empty
                }))
            .Should().ThrowAsync<HttpRequestException>()
            .Where(exception => exception.StatusCode == HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task CreateProjectDisallowsEmptyDisplayName()
    {
        this.ActAsAdmin();

        await Invoking(() => this.ProjectClient.CreateProjectAsync(
                new ProjectSaveRequest
                {
                    DisplayName = "",
                    Members = ImmutableDictionary<Guid, ProjectMemberRole>.Empty
                }))
            .Should().ThrowAsync<HttpRequestException>()
            .Where(exception => exception.StatusCode == HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateProjectDisallowsTooLongDisplayName()
    {
        this.ActAsAdmin();

        await Invoking(() => this.ProjectClient.CreateProjectAsync(
                new ProjectSaveRequest
                {
                    DisplayName = string.Concat(Enumerable.Repeat("a", 256)),
                    Members = ImmutableDictionary<Guid, ProjectMemberRole>.Empty
                }))
            .Should().ThrowAsync<HttpRequestException>()
            .Where(exception => exception.StatusCode == HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateProjectDisallowsInvalidRole()
    {
        this.ActAsAdmin();

        await Invoking(() => this.ProjectClient.CreateProjectAsync(
                new ProjectSaveRequest
                {
                    DisplayName = "DisplayName",
                    Members = new Dictionary<Guid, ProjectMemberRole> { { DefaultUserGuid, (ProjectMemberRole)(-1) } }
                        .ToImmutableDictionary()
                }))
            .Should().ThrowAsync<HttpRequestException>()
            .Where(exception => exception.StatusCode == HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateProjectDisallowsInvalidUserId()
    {
        this.ActAsAdmin();

        await Invoking(() => this.ProjectClient.CreateProjectAsync(
                new ProjectSaveRequest
                {
                    DisplayName = "DisplayName",
                    Members = new Dictionary<Guid, ProjectMemberRole> { { Guid.Empty, ProjectMemberRole.Guest } }
                        .ToImmutableDictionary()
                }))
            .Should().ThrowAsync<HttpRequestException>()
            .Where(exception => exception.StatusCode == HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetProjectDetailsAsAdministrator()
    {
        this.ActAsAdmin();

        var project = await this.ProjectClient.CreateProjectAsync(new ProjectSaveRequest
        {
            DisplayName = "DisplayName",
            Members = new Dictionary<Guid, ProjectMemberRole> { { DefaultUserGuid, ProjectMemberRole.Manager } }
                .ToImmutableDictionary()
        });

        var projectDetailsAsync = await this.ProjectClient.GetProjectDetailsAsync(project.Id);

        projectDetailsAsync.Should().Be(project);
    }

    [Fact]
    public async Task GetProjectDetailsAsNormalUserWithProjectMembership()
    {
        this.ActAsAdmin();

        var project = await this.ProjectClient.CreateProjectAsync(new ProjectSaveRequest
        {
            DisplayName = "DisplayName",
            Members = new Dictionary<Guid, ProjectMemberRole> { { DefaultUserGuid, ProjectMemberRole.Manager } }
                .ToImmutableDictionary()
        });

        this.ActAsUser();

        var projectDetailsAsync = await this.ProjectClient.GetProjectDetailsAsync(project.Id);

        projectDetailsAsync.Should().Be(project);
    }

    [Fact]
    public async Task GetProjectDetailsWithArchivedProject()
    {
        this.ActAsAdmin();

        var project = await this.ProjectClient.CreateProjectAsync(new ProjectSaveRequest
        {
            DisplayName = "DisplayName",
            Members = new Dictionary<Guid, ProjectMemberRole> { { DefaultUserGuid, ProjectMemberRole.Manager } }
                .ToImmutableDictionary()
        });

        var archiveProject = await this.ProjectClient.ArchiveProjectAsync(project.Id);

        var projectDetailsAsync = await this.ProjectClient.GetProjectDetailsAsync(project.Id);

        projectDetailsAsync.Should().Be(archiveProject);
    }

    [Fact]
    public async Task GetProjectDetailsIsForbiddenWhenUserIsNotProjectMember()
    {
        this.ActAsAdmin();

        var project = await this.ProjectClient.CreateProjectAsync(new ProjectSaveRequest
        {
            DisplayName = "DisplayName",
            Members = ImmutableDictionary<Guid, ProjectMemberRole>.Empty
        });

        this.ActAsUser();

        await Invoking(() => this.ProjectClient.GetProjectDetailsAsync(project.Id))
            .Should().ThrowAsync<HttpRequestException>()
            .Where(exception => exception.StatusCode == HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetProjectDetailsWithUnknownProjectId() =>
        await Invoking(() => this.ProjectClient.GetProjectDetailsAsync(Guid.Empty))
            .Should().ThrowAsync<HttpRequestException>()
            .Where(exception => exception.StatusCode == HttpStatusCode.NotFound);

    [Fact]
    public async Task UpdateProjectAsAdministrator()
    {
        this.ActAsAdmin();

        var project = await this.ProjectClient.CreateProjectAsync(new ProjectSaveRequest
        {
            DisplayName = "Before",
            Members = new Dictionary<Guid, ProjectMemberRole> { { DefaultUserGuid, ProjectMemberRole.Manager } }
                .ToImmutableDictionary()
        });

        var updatedProject = await this.ProjectClient.UpdateProjectAsync(
            project.Id,
            new ProjectSaveRequest
            {
                DisplayName = "After",
                Members = new Dictionary<Guid, ProjectMemberRole> { { DefaultUserGuid, ProjectMemberRole.Guest } }
                    .ToImmutableDictionary()
            });

        updatedProject.DisplayName.Should().Be("After");
        updatedProject.Archived.Should().BeFalse();
        updatedProject.Members.Should().SatisfyRespectively(member =>
        {
            var (memberUserId, memberRole) = member;
            memberUserId.Should().Be(DefaultUserGuid);
            memberRole.Should().Be(ProjectMemberRole.Guest);
        });

        var projectDetailsAsync = await this.ProjectClient.GetProjectDetailsAsync(project.Id);

        projectDetailsAsync.Should().Be(updatedProject);
    }

    [Fact]
    public async Task UpdateProjectAsManager()
    {
        this.ActAsAdmin();

        var project = await this.ProjectClient.CreateProjectAsync(new ProjectSaveRequest
        {
            DisplayName = "Before",
            Members = new Dictionary<Guid, ProjectMemberRole> { { DefaultUserGuid, ProjectMemberRole.Manager } }
                .ToImmutableDictionary()
        });

        this.ActAsUser();

        var updatedProject = await this.ProjectClient.UpdateProjectAsync(
            project.Id,
            new ProjectSaveRequest
            {
                DisplayName = "After",
                Members = new Dictionary<Guid, ProjectMemberRole> { { DefaultUserGuid, ProjectMemberRole.Guest } }
                    .ToImmutableDictionary()
            });

        updatedProject.DisplayName.Should().Be("After");
        updatedProject.Archived.Should().BeFalse();
        updatedProject.Members.Should().SatisfyRespectively(member =>
        {
            var (memberUserId, memberRole) = member;
            memberUserId.Should().Be(DefaultUserGuid);
            memberRole.Should().Be(ProjectMemberRole.Guest);
        });

        var projectDetailsAsync = await this.ProjectClient.GetProjectDetailsAsync(project.Id);

        projectDetailsAsync.Should().Be(updatedProject);
    }

    [Fact]
    public async Task UpdateProjectIsForbiddenWhenUserIsNotProjectManager()
    {
        this.ActAsAdmin();

        var project = await this.ProjectClient.CreateProjectAsync(new ProjectSaveRequest
        {
            DisplayName = "DisplayName",
            Members = new Dictionary<Guid, ProjectMemberRole> { { DefaultUserGuid, ProjectMemberRole.Worker } }
                .ToImmutableDictionary()
        });

        this.ActAsUser();

        await Invoking(() => this.ProjectClient.UpdateProjectAsync(
                project.Id,
                new ProjectSaveRequest
                {
                    DisplayName = "DisplayName",
                    Members = ImmutableDictionary<Guid, ProjectMemberRole>.Empty
                }))
            .Should().ThrowAsync<HttpRequestException>()
            .Where(exception => exception.StatusCode == HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task UpdateProjectNotPossibleWhenProjectIsArchived()
    {
        this.ActAsAdmin();

        var project = await this.ProjectClient.CreateProjectAsync(new ProjectSaveRequest
        {
            DisplayName = "DisplayName",
            Members = new Dictionary<Guid, ProjectMemberRole> { { DefaultUserGuid, ProjectMemberRole.Manager } }
                .ToImmutableDictionary()
        });

        await this.ProjectClient.ArchiveProjectAsync(project.Id);

        await Invoking(() => this.ProjectClient.UpdateProjectAsync(
                project.Id,
                new ProjectSaveRequest
                {
                    DisplayName = "DisplayName",
                    Members = ImmutableDictionary<Guid, ProjectMemberRole>.Empty
                }))
            .Should().ThrowAsync<HttpRequestException>()
            .Where(exception => exception.StatusCode == HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdateProjectDisallowsEmptyDisplayName()
    {
        this.ActAsAdmin();

        var project = await this.ProjectClient.CreateProjectAsync(new ProjectSaveRequest
        {
            DisplayName = "DisplayName",
            Members = ImmutableDictionary<Guid, ProjectMemberRole>.Empty
        });

        await Invoking(() => this.ProjectClient.UpdateProjectAsync(
                project.Id,
                new ProjectSaveRequest
                {
                    DisplayName = "",
                    Members = ImmutableDictionary<Guid, ProjectMemberRole>.Empty
                }))
            .Should().ThrowAsync<HttpRequestException>()
            .Where(exception => exception.StatusCode == HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdateProjectDisallowsTooLongDisplayName()
    {
        this.ActAsAdmin();

        var project = await this.ProjectClient.CreateProjectAsync(new ProjectSaveRequest
        {
            DisplayName = "DisplayName",
            Members = ImmutableDictionary<Guid, ProjectMemberRole>.Empty
        });

        await Invoking(() => this.ProjectClient.UpdateProjectAsync(
                project.Id,
                new ProjectSaveRequest
                {
                    DisplayName = string.Concat(Enumerable.Repeat("a", 256)),
                    Members = ImmutableDictionary<Guid, ProjectMemberRole>.Empty
                }))
            .Should().ThrowAsync<HttpRequestException>()
            .Where(exception => exception.StatusCode == HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdateProjectDisallowsInvalidRole()
    {
        this.ActAsAdmin();

        var project = await this.ProjectClient.CreateProjectAsync(new ProjectSaveRequest
        {
            DisplayName = "DisplayName",
            Members = ImmutableDictionary<Guid, ProjectMemberRole>.Empty
        });

        await Invoking(() => this.ProjectClient.UpdateProjectAsync(
                project.Id,
                new ProjectSaveRequest
                {
                    DisplayName = "DisplayName",
                    Members = new Dictionary<Guid, ProjectMemberRole> { { DefaultUserGuid, (ProjectMemberRole)(-1) } }
                        .ToImmutableDictionary()
                }))
            .Should().ThrowAsync<HttpRequestException>()
            .Where(exception => exception.StatusCode == HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdateProjectDisallowsInvalidUserId()
    {
        this.ActAsAdmin();

        var project = await this.ProjectClient.CreateProjectAsync(new ProjectSaveRequest
        {
            DisplayName = "DisplayName",
            Members = ImmutableDictionary<Guid, ProjectMemberRole>.Empty
        });

        await Invoking(() => this.ProjectClient.UpdateProjectAsync(
                project.Id,
                new ProjectSaveRequest
                {
                    DisplayName = "DisplayName",
                    Members = new Dictionary<Guid, ProjectMemberRole> { { Guid.Empty, ProjectMemberRole.Guest } }
                        .ToImmutableDictionary()
                }))
            .Should().ThrowAsync<HttpRequestException>()
            .Where(exception => exception.StatusCode == HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdateProjectWithUnknownProjectId() =>
        await Invoking(() => this.ProjectClient.UpdateProjectAsync(
                Guid.Empty,
                new ProjectSaveRequest
                {
                    DisplayName = "DisplayName",
                    Members = ImmutableDictionary<Guid, ProjectMemberRole>.Empty
                }))
            .Should().ThrowAsync<HttpRequestException>()
            .Where(exception => exception.StatusCode == HttpStatusCode.NotFound);

    [Fact]
    public async Task ArchiveProjectAsAdministrator()
    {
        this.ActAsAdmin();

        var project = await this.ProjectClient.CreateProjectAsync(new ProjectSaveRequest
        {
            DisplayName = "DisplayName",
            Members = new Dictionary<Guid, ProjectMemberRole> { { DefaultUserGuid, ProjectMemberRole.Manager } }
                .ToImmutableDictionary()
        });

        var archivedProject = await this.ProjectClient.ArchiveProjectAsync(project.Id);

        archivedProject.DisplayName.Should().Be("DisplayName");
        archivedProject.Archived.Should().BeTrue();
        archivedProject.Members.Should().SatisfyRespectively(member =>
        {
            var (memberUserId, memberRole) = member;
            memberUserId.Should().Be(DefaultUserGuid);
            memberRole.Should().Be(ProjectMemberRole.Manager);
        });
    }

    [Fact]
    public async Task ArchiveProjectAsManager()
    {
        this.ActAsAdmin();

        var project = await this.ProjectClient.CreateProjectAsync(new ProjectSaveRequest
        {
            DisplayName = "DisplayName",
            Members = new Dictionary<Guid, ProjectMemberRole> { { DefaultUserGuid, ProjectMemberRole.Manager } }
                .ToImmutableDictionary()
        });

        this.ActAsUser();

        var archivedProject = await this.ProjectClient.ArchiveProjectAsync(project.Id);

        archivedProject.DisplayName.Should().Be("DisplayName");
        archivedProject.Archived.Should().BeTrue();
        archivedProject.Members.Should().SatisfyRespectively(member =>
        {
            var (memberUserId, memberRole) = member;
            memberUserId.Should().Be(DefaultUserGuid);
            memberRole.Should().Be(ProjectMemberRole.Manager);
        });
    }

    [Fact]
    public async Task ArchiveProjectIsForbiddenWhenUserIsNotProjectManager()
    {
        this.ActAsAdmin();

        var project = await this.ProjectClient.CreateProjectAsync(new ProjectSaveRequest
        {
            DisplayName = "DisplayName",
            Members = new Dictionary<Guid, ProjectMemberRole> { { DefaultUserGuid, ProjectMemberRole.Worker } }
                .ToImmutableDictionary()
        });

        this.ActAsUser();

        await Invoking(() => this.ProjectClient.ArchiveProjectAsync(project.Id))
            .Should().ThrowAsync<HttpRequestException>()
            .Where(exception => exception.StatusCode == HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task ArchiveProjectIsNotPossibleWhenProjectAlreadyArchived()
    {
        this.ActAsAdmin();

        var project = await this.ProjectClient.CreateProjectAsync(new ProjectSaveRequest
        {
            DisplayName = "DisplayName",
            Members = new Dictionary<Guid, ProjectMemberRole> { { DefaultUserGuid, ProjectMemberRole.Worker } }
                .ToImmutableDictionary()
        });

        await this.ProjectClient.ArchiveProjectAsync(project.Id);

        await Invoking(() => this.ProjectClient.ArchiveProjectAsync(project.Id))
            .Should().ThrowAsync<HttpRequestException>()
            .Where(exception => exception.StatusCode == HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task ArchiveProjectWithUnknownProjectId() =>
        await Invoking(() => this.ProjectClient.ArchiveProjectAsync(Guid.Empty))
            .Should().ThrowAsync<HttpRequestException>()
            .Where(exception => exception.StatusCode == HttpStatusCode.NotFound);

    [Fact]
    public async Task RestoreProjectAsAdministrator()
    {
        this.ActAsAdmin();

        var project = await this.ProjectClient.CreateProjectAsync(new ProjectSaveRequest
        {
            DisplayName = "DisplayName",
            Members = new Dictionary<Guid, ProjectMemberRole> { { DefaultUserGuid, ProjectMemberRole.Manager } }
                .ToImmutableDictionary()
        });

        await this.ProjectClient.ArchiveProjectAsync(project.Id);
        var restoredProject = await this.ProjectClient.RestoreProjectAsync(project.Id);

        restoredProject.Should().Be(project);
    }

    [Fact]
    public async Task RestoreProjectAsManager()
    {
        this.ActAsAdmin();

        var project = await this.ProjectClient.CreateProjectAsync(new ProjectSaveRequest
        {
            DisplayName = "DisplayName",
            Members = new Dictionary<Guid, ProjectMemberRole> { { DefaultUserGuid, ProjectMemberRole.Manager } }
                .ToImmutableDictionary()
        });

        await this.ProjectClient.ArchiveProjectAsync(project.Id);

        this.ActAsUser();

        var restoredProject = await this.ProjectClient.RestoreProjectAsync(project.Id);

        restoredProject.Should().Be(project);
    }

    [Fact]
    public async Task RestoreProjectIsForbiddenWhenUserIsNotProjectManager()
    {
        this.ActAsAdmin();

        var project = await this.ProjectClient.CreateProjectAsync(new ProjectSaveRequest
        {
            DisplayName = "DisplayName",
            Members = new Dictionary<Guid, ProjectMemberRole> { { DefaultUserGuid, ProjectMemberRole.Worker } }
                .ToImmutableDictionary()
        });

        await this.ProjectClient.ArchiveProjectAsync(project.Id);

        this.ActAsUser();

        await Invoking(() => this.ProjectClient.RestoreProjectAsync(project.Id))
            .Should().ThrowAsync<HttpRequestException>()
            .Where(exception => exception.StatusCode == HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task RestoreProjectIsNotPossibleWhenProjectNotArchived()
    {
        this.ActAsAdmin();

        var project = await this.ProjectClient.CreateProjectAsync(new ProjectSaveRequest
        {
            DisplayName = "DisplayName",
            Members = new Dictionary<Guid, ProjectMemberRole> { { DefaultUserGuid, ProjectMemberRole.Worker } }
                .ToImmutableDictionary()
        });

        await Invoking(() => this.ProjectClient.RestoreProjectAsync(project.Id))
            .Should().ThrowAsync<HttpRequestException>()
            .Where(exception => exception.StatusCode == HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task RestoreProjectWithUnknownProjectId() =>
        await Invoking(() => this.ProjectClient.RestoreProjectAsync(Guid.Empty))
            .Should().ThrowAsync<HttpRequestException>()
            .Where(exception => exception.StatusCode == HttpStatusCode.NotFound);
}
