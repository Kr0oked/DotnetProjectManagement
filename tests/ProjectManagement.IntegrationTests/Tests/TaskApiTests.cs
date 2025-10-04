namespace DotnetProjectManagement.ProjectManagement.IntegrationTests.Tests;

using System.Collections.Immutable;
using System.Net;
using Domain.Actions;
using Domain.Entities;
using FluentAssertions;
using Moq;
using UseCases.DTOs;
using Web.Models;
using Xunit;
using Xunit.Abstractions;
using static FluentAssertions.FluentActions;

public class TaskApiTests(TestWebApplicationFactory<Program> testWebApplicationFactory, ITestOutputHelper output)
    : IntegrationTest(testWebApplicationFactory, output)
{
    [Fact]
    public async Task CreateTaskAsAdministrator()
    {
        this.ActAsAdmin();

        var project = await this.ProjectClient.CreateProjectAsync(new ProjectSaveRequest
        {
            DisplayName = "ProjectDisplayName",
            Members = ImmutableDictionary<Guid, ProjectMemberRole>.Empty
        });

        this.MessageBrokerMock.Reset();

        var task = await this.TaskClient.CreateTaskAsync(new TaskCreateRequest
        {
            ProjectId = project.Id,
            DisplayName = "DisplayName",
            Description = "Description",
            Assignees = [DefaultUserGuid]
        });

        task.DisplayName.Should().Be("DisplayName");
        task.Description.Should().Be("Description");
        task.Open.Should().BeTrue();
        task.Assignees.Should().Equal(DefaultUserGuid);

        var capturedMessages = new List<TaskActionMessage>();
        this.MessageBrokerMock.Verify(messageBroker => messageBroker
            .Publish(Capture.In(capturedMessages), It.IsAny<CancellationToken>()));
        capturedMessages.Should().SatisfyRespectively(message =>
        {
            message.ActorUserId.Should().Be(DefaultAdminGuid);
            message.Action.Should().Be(TaskAction.Create);
            message.Task.Should().BeEquivalentTo(task);
            message.Project.Should().BeEquivalentTo(project);
        });
    }

    [Fact]
    public async Task CreateTaskAsManager()
    {
        this.ActAsAdmin();

        var project = await this.ProjectClient.CreateProjectAsync(new ProjectSaveRequest
        {
            DisplayName = "ProjectDisplayName",
            Members = new Dictionary<Guid, ProjectMemberRole> { { DefaultUserGuid, ProjectMemberRole.Manager } }
                .ToImmutableDictionary()
        });

        this.ActAsAdmin();
        this.MessageBrokerMock.Reset();

        var task = await this.TaskClient.CreateTaskAsync(new TaskCreateRequest
        {
            ProjectId = project.Id,
            DisplayName = "DisplayName",
            Description = "Description",
            Assignees = [DefaultUserGuid]
        });

        task.DisplayName.Should().Be("DisplayName");
        task.Description.Should().Be("Description");
        task.Open.Should().BeTrue();
        task.Assignees.Should().Equal(DefaultUserGuid);

        var capturedMessages = new List<TaskActionMessage>();
        this.MessageBrokerMock.Verify(messageBroker => messageBroker
            .Publish(Capture.In(capturedMessages), It.IsAny<CancellationToken>()));
        capturedMessages.Should().SatisfyRespectively(message =>
        {
            message.ActorUserId.Should().Be(DefaultAdminGuid);
            message.Action.Should().Be(TaskAction.Create);
            message.Task.Should().BeEquivalentTo(task);
            message.Project.Should().BeEquivalentTo(project);
        });
    }

    [Fact]
    public async Task CreateTaskAsWorkerIsForbidden()
    {
        this.ActAsAdmin();

        var project = await this.ProjectClient.CreateProjectAsync(new ProjectSaveRequest
        {
            DisplayName = "ProjectDisplayName",
            Members = new Dictionary<Guid, ProjectMemberRole> { { DefaultUserGuid, ProjectMemberRole.Worker } }
                .ToImmutableDictionary()
        });

        this.ActAsUser();

        await Invoking(() => this.TaskClient.CreateTaskAsync(
                new TaskCreateRequest
                {
                    ProjectId = project.Id,
                    DisplayName = "DisplayName",
                    Description = "Description",
                    Assignees = [DefaultUserGuid]
                }))
            .Should().ThrowAsync<HttpRequestException>()
            .Where(exception => exception.StatusCode == HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task CreateTaskDisallowsTooLongDisplayName()
    {
        this.ActAsAdmin();

        var project = await this.ProjectClient.CreateProjectAsync(new ProjectSaveRequest
        {
            DisplayName = "ProjectDisplayName",
            Members = ImmutableDictionary<Guid, ProjectMemberRole>.Empty
        });

        await Invoking(() => this.TaskClient.CreateTaskAsync(
                new TaskCreateRequest
                {
                    ProjectId = project.Id,
                    DisplayName = string.Concat(Enumerable.Repeat("a", 256)),
                    Description = "Description",
                    Assignees = [DefaultUserGuid]
                }))
            .Should().ThrowAsync<HttpRequestException>()
            .Where(exception => exception.StatusCode == HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateTaskDisallowsInvalidUserId()
    {
        this.ActAsAdmin();

        var project = await this.ProjectClient.CreateProjectAsync(new ProjectSaveRequest
        {
            DisplayName = "ProjectDisplayName",
            Members = ImmutableDictionary<Guid, ProjectMemberRole>.Empty
        });

        await Invoking(() => this.TaskClient.CreateTaskAsync(
                new TaskCreateRequest
                {
                    ProjectId = project.Id,
                    DisplayName = "DisplayName",
                    Description = "Description",
                    Assignees = [Guid.Empty]
                }))
            .Should().ThrowAsync<HttpRequestException>()
            .Where(exception => exception.StatusCode == HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateTaskNotPossibleWhenProjectIsArchived()
    {
        this.ActAsAdmin();

        var project = await this.ProjectClient.CreateProjectAsync(new ProjectSaveRequest
        {
            DisplayName = "ProjectDisplayName",
            Members = ImmutableDictionary<Guid, ProjectMemberRole>.Empty
        });

        await this.ProjectClient.ArchiveProjectAsync(project.Id);

        await Invoking(() => this.TaskClient.CreateTaskAsync(
                new TaskCreateRequest
                {
                    ProjectId = project.Id,
                    DisplayName = "DisplayName",
                    Description = "Description",
                    Assignees = []
                }))
            .Should().ThrowAsync<HttpRequestException>()
            .Where(exception => exception.StatusCode == HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetTaskDetailsAsAdministrator()
    {
        this.ActAsAdmin();

        var project = await this.ProjectClient.CreateProjectAsync(new ProjectSaveRequest
        {
            DisplayName = "ProjectDisplayName",
            Members = ImmutableDictionary<Guid, ProjectMemberRole>.Empty
        });

        var createdTask = await this.TaskClient.CreateTaskAsync(new TaskCreateRequest
        {
            ProjectId = project.Id,
            DisplayName = "DisplayName",
            Description = "Description",
            Assignees = [DefaultUserGuid]
        });

        var task = await this.TaskClient.GetTaskDetailsAsync(createdTask.Id);

        task.Should().Be(createdTask);
    }

    [Fact]
    public async Task GetTaskDetailsAsNormalUserWithProjectMembership()
    {
        this.ActAsAdmin();

        var project = await this.ProjectClient.CreateProjectAsync(new ProjectSaveRequest
        {
            DisplayName = "ProjectDisplayName",
            Members = new Dictionary<Guid, ProjectMemberRole> { { DefaultUserGuid, ProjectMemberRole.Manager } }
                .ToImmutableDictionary()
        });

        var createdTask = await this.TaskClient.CreateTaskAsync(new TaskCreateRequest
        {
            ProjectId = project.Id,
            DisplayName = "DisplayName",
            Description = "Description",
            Assignees = []
        });

        this.ActAsUser();

        var task = await this.TaskClient.GetTaskDetailsAsync(createdTask.Id);

        task.Should().Be(createdTask);
    }

    [Fact]
    public async Task GetTaskDetailsWithArchivedProject()
    {
        this.ActAsAdmin();

        var project = await this.ProjectClient.CreateProjectAsync(new ProjectSaveRequest
        {
            DisplayName = "ProjectDisplayName",
            Members = new Dictionary<Guid, ProjectMemberRole> { { DefaultUserGuid, ProjectMemberRole.Manager } }
                .ToImmutableDictionary()
        });

        var createdTask = await this.TaskClient.CreateTaskAsync(new TaskCreateRequest
        {
            ProjectId = project.Id,
            DisplayName = "DisplayName",
            Description = "Description",
            Assignees = []
        });

        await this.ProjectClient.ArchiveProjectAsync(project.Id);

        this.ActAsUser();

        var task = await this.TaskClient.GetTaskDetailsAsync(createdTask.Id);

        task.Should().Be(createdTask);
    }

    [Fact]
    public async Task GetTaskDetailsIsForbiddenWhenUserIsNotProjectMember()
    {
        this.ActAsAdmin();

        var project = await this.ProjectClient.CreateProjectAsync(new ProjectSaveRequest
        {
            DisplayName = "ProjectDisplayName",
            Members = ImmutableDictionary<Guid, ProjectMemberRole>.Empty
        });

        var task = await this.TaskClient.CreateTaskAsync(new TaskCreateRequest
        {
            ProjectId = project.Id,
            DisplayName = "DisplayName",
            Description = "Description",
            Assignees = []
        });

        this.ActAsUser();

        await Invoking(() => this.TaskClient.GetTaskDetailsAsync(task.Id))
            .Should().ThrowAsync<HttpRequestException>()
            .Where(exception => exception.StatusCode == HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetTaskDetailsWithUnknownTaskId()
    {
        this.ActAsAdmin();

        await Invoking(() => this.TaskClient.GetTaskDetailsAsync(Guid.Empty))
            .Should().ThrowAsync<HttpRequestException>()
            .Where(exception => exception.StatusCode == HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UpdateTaskAsAdministrator()
    {
        this.ActAsAdmin();

        var project = await this.ProjectClient.CreateProjectAsync(new ProjectSaveRequest
        {
            DisplayName = "ProjectDisplayName",
            Members = ImmutableDictionary<Guid, ProjectMemberRole>.Empty
        });

        var task = await this.TaskClient.CreateTaskAsync(new TaskCreateRequest
        {
            ProjectId = project.Id,
            DisplayName = "DisplayNameBefore",
            Description = "DescriptionBefore",
            Assignees = [DefaultUserGuid]
        });

        this.MessageBrokerMock.Reset();

        var updatedTask = await this.TaskClient.UpdateTaskAsync(
            task.Id,
            new TaskUpdateRequest
            {
                DisplayName = "DisplayNameAfter",
                Description = "DescriptionAfter",
                Assignees = [DefaultUserGuid]
            });

        updatedTask.DisplayName.Should().Be("DisplayNameAfter");
        updatedTask.Description.Should().Be("DescriptionAfter");
        updatedTask.Open.Should().BeTrue();
        updatedTask.Assignees.Should().Equal(DefaultUserGuid);

        var capturedMessages = new List<TaskActionMessage>();
        this.MessageBrokerMock.Verify(messageBroker => messageBroker
            .Publish(Capture.In(capturedMessages), It.IsAny<CancellationToken>()));
        capturedMessages.Should().SatisfyRespectively(message =>
        {
            message.ActorUserId.Should().Be(DefaultAdminGuid);
            message.Action.Should().Be(TaskAction.Update);
            message.Task.Should().BeEquivalentTo(updatedTask);
            message.Project.Should().BeEquivalentTo(project);
        });

        var taskDetails = await this.TaskClient.GetTaskDetailsAsync(task.Id);

        taskDetails.Should().Be(updatedTask);
    }

    [Fact]
    public async Task UpdateTaskAsManager()
    {
        this.ActAsAdmin();

        var project = await this.ProjectClient.CreateProjectAsync(new ProjectSaveRequest
        {
            DisplayName = "ProjectDisplayName",
            Members = new Dictionary<Guid, ProjectMemberRole> { { DefaultUserGuid, ProjectMemberRole.Manager } }
                .ToImmutableDictionary()
        });

        var task = await this.TaskClient.CreateTaskAsync(new TaskCreateRequest
        {
            ProjectId = project.Id,
            DisplayName = "DisplayNameBefore",
            Description = "DescriptionBefore",
            Assignees = [DefaultUserGuid]
        });

        this.ActAsUser();
        this.MessageBrokerMock.Reset();

        var updatedTask = await this.TaskClient.UpdateTaskAsync(
            task.Id,
            new TaskUpdateRequest
            {
                DisplayName = "DisplayNameAfter",
                Description = "DescriptionAfter",
                Assignees = [DefaultUserGuid]
            });

        updatedTask.DisplayName.Should().Be("DisplayNameAfter");
        updatedTask.Description.Should().Be("DescriptionAfter");
        updatedTask.Open.Should().BeTrue();
        updatedTask.Assignees.Should().Equal(DefaultUserGuid);

        var capturedMessages = new List<TaskActionMessage>();
        this.MessageBrokerMock.Verify(messageBroker => messageBroker
            .Publish(Capture.In(capturedMessages), It.IsAny<CancellationToken>()));
        capturedMessages.Should().SatisfyRespectively(message =>
        {
            message.ActorUserId.Should().Be(DefaultUserGuid);
            message.Action.Should().Be(TaskAction.Update);
            message.Task.Should().BeEquivalentTo(updatedTask);
            message.Project.Should().BeEquivalentTo(project);
        });

        var taskDetails = await this.TaskClient.GetTaskDetailsAsync(task.Id);

        taskDetails.Should().Be(updatedTask);
    }

    [Fact]
    public async Task UpdateTaskIsForbiddenWhenUserIsNotProjectManager()
    {
        this.ActAsAdmin();

        var project = await this.ProjectClient.CreateProjectAsync(new ProjectSaveRequest
        {
            DisplayName = "ProjectDisplayName",
            Members = ImmutableDictionary<Guid, ProjectMemberRole>.Empty
        });

        var task = await this.TaskClient.CreateTaskAsync(new TaskCreateRequest
        {
            ProjectId = project.Id,
            DisplayName = "DisplayNameBefore",
            Description = "DescriptionBefore",
            Assignees = [DefaultUserGuid]
        });

        this.ActAsUser();

        await Invoking(() => this.TaskClient.UpdateTaskAsync(
                task.Id,
                new TaskUpdateRequest
                {
                    DisplayName = "DisplayNameAfter",
                    Description = "DescriptionAfter",
                    Assignees = [DefaultUserGuid]
                }))
            .Should().ThrowAsync<HttpRequestException>()
            .Where(exception => exception.StatusCode == HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task UpdateTaskIsNotPossibleWhenProjectIsArchived()
    {
        this.ActAsAdmin();

        var project = await this.ProjectClient.CreateProjectAsync(new ProjectSaveRequest
        {
            DisplayName = "ProjectDisplayName",
            Members = ImmutableDictionary<Guid, ProjectMemberRole>.Empty
        });

        var task = await this.TaskClient.CreateTaskAsync(new TaskCreateRequest
        {
            ProjectId = project.Id,
            DisplayName = "DisplayNameBefore",
            Description = "DescriptionBefore",
            Assignees = [DefaultUserGuid]
        });

        await this.ProjectClient.ArchiveProjectAsync(project.Id);

        await Invoking(() => this.TaskClient.UpdateTaskAsync(
                task.Id,
                new TaskUpdateRequest
                {
                    DisplayName = "DisplayNameAfter",
                    Description = "DescriptionAfter",
                    Assignees = [DefaultUserGuid]
                }))
            .Should().ThrowAsync<HttpRequestException>()
            .Where(exception => exception.StatusCode == HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdateTaskDisallowsEmptyDisplayName()
    {
        this.ActAsAdmin();

        var project = await this.ProjectClient.CreateProjectAsync(new ProjectSaveRequest
        {
            DisplayName = "ProjectDisplayName",
            Members = ImmutableDictionary<Guid, ProjectMemberRole>.Empty
        });

        var task = await this.TaskClient.CreateTaskAsync(new TaskCreateRequest
        {
            ProjectId = project.Id,
            DisplayName = "DisplayNameBefore",
            Description = "DescriptionBefore",
            Assignees = [DefaultUserGuid]
        });

        await Invoking(() => this.TaskClient.UpdateTaskAsync(
                task.Id,
                new TaskUpdateRequest
                {
                    DisplayName = "",
                    Description = "DescriptionAfter",
                    Assignees = [DefaultUserGuid]
                }))
            .Should().ThrowAsync<HttpRequestException>()
            .Where(exception => exception.StatusCode == HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdateTaskDisallowsTooLongDisplayName()
    {
        this.ActAsAdmin();

        var project = await this.ProjectClient.CreateProjectAsync(new ProjectSaveRequest
        {
            DisplayName = "ProjectDisplayName",
            Members = ImmutableDictionary<Guid, ProjectMemberRole>.Empty
        });

        var task = await this.TaskClient.CreateTaskAsync(new TaskCreateRequest
        {
            ProjectId = project.Id,
            DisplayName = "DisplayNameBefore",
            Description = "DescriptionBefore",
            Assignees = [DefaultUserGuid]
        });

        await Invoking(() => this.TaskClient.UpdateTaskAsync(
                task.Id,
                new TaskUpdateRequest
                {
                    DisplayName = string.Concat(Enumerable.Repeat("a", 256)),
                    Description = "DescriptionAfter",
                    Assignees = [DefaultUserGuid]
                }))
            .Should().ThrowAsync<HttpRequestException>()
            .Where(exception => exception.StatusCode == HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdateTaskDisallowsInvalidUserId()
    {
        this.ActAsAdmin();

        var project = await this.ProjectClient.CreateProjectAsync(new ProjectSaveRequest
        {
            DisplayName = "ProjectDisplayName",
            Members = ImmutableDictionary<Guid, ProjectMemberRole>.Empty
        });

        var task = await this.TaskClient.CreateTaskAsync(new TaskCreateRequest
        {
            ProjectId = project.Id,
            DisplayName = "DisplayNameBefore",
            Description = "DescriptionBefore",
            Assignees = [DefaultUserGuid]
        });

        await Invoking(() => this.TaskClient.UpdateTaskAsync(
                task.Id,
                new TaskUpdateRequest
                {
                    DisplayName = "DisplayNameAfter",
                    Description = "DescriptionAfter",
                    Assignees = [Guid.Empty]
                }))
            .Should().ThrowAsync<HttpRequestException>()
            .Where(exception => exception.StatusCode == HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdateTaskWithInvalidTaskId()
    {
        this.ActAsAdmin();

        await Invoking(() => this.TaskClient.UpdateTaskAsync(
                Guid.Empty,
                new TaskUpdateRequest
                {
                    DisplayName = "DisplayName",
                    Description = "Description",
                    Assignees = []
                }))
            .Should().ThrowAsync<HttpRequestException>()
            .Where(exception => exception.StatusCode == HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task CloseTaskAsAdministrator()
    {
        this.ActAsAdmin();

        var project = await this.ProjectClient.CreateProjectAsync(new ProjectSaveRequest
        {
            DisplayName = "ProjectDisplayName",
            Members = ImmutableDictionary<Guid, ProjectMemberRole>.Empty
        });

        var task = await this.TaskClient.CreateTaskAsync(new TaskCreateRequest
        {
            ProjectId = project.Id,
            DisplayName = "DisplayName",
            Description = "Description",
            Assignees = [DefaultUserGuid]
        });

        this.MessageBrokerMock.Reset();

        var closedTask = await this.TaskClient.CloseTaskAsync(task.Id);

        closedTask.Id.Should().Be(task.Id);
        closedTask.DisplayName.Should().Be("DisplayName");
        closedTask.Description.Should().Be("Description");
        closedTask.Open.Should().BeFalse();
        closedTask.Assignees.Should().Equal(DefaultUserGuid);

        var capturedMessages = new List<TaskActionMessage>();
        this.MessageBrokerMock.Verify(messageBroker => messageBroker
            .Publish(Capture.In(capturedMessages), It.IsAny<CancellationToken>()));
        capturedMessages.Should().SatisfyRespectively(message =>
        {
            message.ActorUserId.Should().Be(DefaultAdminGuid);
            message.Action.Should().Be(TaskAction.Close);
            message.Task.Should().BeEquivalentTo(closedTask);
            message.Project.Should().BeEquivalentTo(project);
        });

        var taskDetails = await this.TaskClient.GetTaskDetailsAsync(task.Id);

        taskDetails.Should().Be(closedTask);
    }

    [Fact]
    public async Task CloseTaskAsManager()
    {
        this.ActAsAdmin();

        var project = await this.ProjectClient.CreateProjectAsync(new ProjectSaveRequest
        {
            DisplayName = "ProjectDisplayName",
            Members = new Dictionary<Guid, ProjectMemberRole> { { DefaultUserGuid, ProjectMemberRole.Manager } }
                .ToImmutableDictionary()
        });

        var task = await this.TaskClient.CreateTaskAsync(new TaskCreateRequest
        {
            ProjectId = project.Id,
            DisplayName = "DisplayName",
            Description = "Description",
            Assignees = []
        });

        this.ActAsUser();
        this.MessageBrokerMock.Reset();

        var closedTask = await this.TaskClient.CloseTaskAsync(task.Id);

        closedTask.Id.Should().Be(task.Id);
        closedTask.DisplayName.Should().Be("DisplayName");
        closedTask.Description.Should().Be("Description");
        closedTask.Open.Should().BeFalse();
        closedTask.Assignees.Should().Equal();

        var capturedMessages = new List<TaskActionMessage>();
        this.MessageBrokerMock.Verify(messageBroker => messageBroker
            .Publish(Capture.In(capturedMessages), It.IsAny<CancellationToken>()));
        capturedMessages.Should().SatisfyRespectively(message =>
        {
            message.ActorUserId.Should().Be(DefaultUserGuid);
            message.Action.Should().Be(TaskAction.Close);
            message.Task.Should().BeEquivalentTo(closedTask);
            message.Project.Should().BeEquivalentTo(project);
        });

        var taskDetails = await this.TaskClient.GetTaskDetailsAsync(task.Id);

        taskDetails.Should().Be(closedTask);
    }

    [Fact]
    public async Task CloseTaskAsAssignee()
    {
        this.ActAsAdmin();

        var project = await this.ProjectClient.CreateProjectAsync(new ProjectSaveRequest
        {
            DisplayName = "ProjectDisplayName",
            Members = ImmutableDictionary<Guid, ProjectMemberRole>.Empty
        });

        var task = await this.TaskClient.CreateTaskAsync(new TaskCreateRequest
        {
            ProjectId = project.Id,
            DisplayName = "DisplayName",
            Description = "Description",
            Assignees = [DefaultUserGuid]
        });

        this.ActAsUser();
        this.MessageBrokerMock.Reset();

        var closedTask = await this.TaskClient.CloseTaskAsync(task.Id);

        closedTask.Id.Should().Be(task.Id);
        closedTask.DisplayName.Should().Be("DisplayName");
        closedTask.Description.Should().Be("Description");
        closedTask.Open.Should().BeFalse();
        closedTask.Assignees.Should().Equal(DefaultUserGuid);

        var capturedMessages = new List<TaskActionMessage>();
        this.MessageBrokerMock.Verify(messageBroker => messageBroker
            .Publish(Capture.In(capturedMessages), It.IsAny<CancellationToken>()));
        capturedMessages.Should().SatisfyRespectively(message =>
        {
            message.ActorUserId.Should().Be(DefaultUserGuid);
            message.Action.Should().Be(TaskAction.Close);
            message.Task.Should().BeEquivalentTo(closedTask);
            message.Project.Should().BeEquivalentTo(project);
        });
    }

    [Fact]
    public async Task CloseTaskIsForbiddenWhenUserIsNotAssigned()
    {
        this.ActAsAdmin();

        var project = await this.ProjectClient.CreateProjectAsync(new ProjectSaveRequest
        {
            DisplayName = "ProjectDisplayName",
            Members = new Dictionary<Guid, ProjectMemberRole> { { DefaultUserGuid, ProjectMemberRole.Worker } }
                .ToImmutableDictionary()
        });

        var task = await this.TaskClient.CreateTaskAsync(new TaskCreateRequest
        {
            ProjectId = project.Id,
            DisplayName = "DisplayName",
            Description = "Description",
            Assignees = []
        });

        this.ActAsUser();

        await Invoking(() => this.TaskClient.CloseTaskAsync(task.Id))
            .Should().ThrowAsync<HttpRequestException>()
            .Where(exception => exception.StatusCode == HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task CloseTaskIsForbiddenWhenTaskIsAlreadyClosed()
    {
        this.ActAsAdmin();

        var project = await this.ProjectClient.CreateProjectAsync(new ProjectSaveRequest
        {
            DisplayName = "ProjectDisplayName",
            Members = ImmutableDictionary<Guid, ProjectMemberRole>.Empty
        });

        var task = await this.TaskClient.CreateTaskAsync(new TaskCreateRequest
        {
            ProjectId = project.Id,
            DisplayName = "DisplayName",
            Description = "Description",
            Assignees = []
        });

        await this.TaskClient.CloseTaskAsync(task.Id);

        await Invoking(() => this.TaskClient.CloseTaskAsync(task.Id))
            .Should().ThrowAsync<HttpRequestException>()
            .Where(exception => exception.StatusCode == HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CloseTaskNotPossibleWhenProjectIsArchived()
    {
        this.ActAsAdmin();

        var project = await this.ProjectClient.CreateProjectAsync(new ProjectSaveRequest
        {
            DisplayName = "ProjectDisplayName",
            Members = ImmutableDictionary<Guid, ProjectMemberRole>.Empty
        });

        var task = await this.TaskClient.CreateTaskAsync(new TaskCreateRequest
        {
            ProjectId = project.Id,
            DisplayName = "DisplayName",
            Description = "Description",
            Assignees = []
        });

        await this.ProjectClient.ArchiveProjectAsync(project.Id);

        await Invoking(() => this.TaskClient.CloseTaskAsync(task.Id))
            .Should().ThrowAsync<HttpRequestException>()
            .Where(exception => exception.StatusCode == HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task ReopenTaskAsAdministrator()
    {
        this.ActAsAdmin();

        var project = await this.ProjectClient.CreateProjectAsync(new ProjectSaveRequest
        {
            DisplayName = "ProjectDisplayName",
            Members = ImmutableDictionary<Guid, ProjectMemberRole>.Empty
        });

        var task = await this.TaskClient.CreateTaskAsync(new TaskCreateRequest
        {
            ProjectId = project.Id,
            DisplayName = "DisplayName",
            Description = "Description",
            Assignees = [DefaultUserGuid]
        });

        await this.TaskClient.CloseTaskAsync(task.Id);

        this.MessageBrokerMock.Reset();

        var reopenedTask = await this.TaskClient.ReopenTaskAsync(task.Id);

        reopenedTask.Id.Should().Be(task.Id);
        reopenedTask.DisplayName.Should().Be("DisplayName");
        reopenedTask.Description.Should().Be("Description");
        reopenedTask.Open.Should().BeTrue();
        reopenedTask.Assignees.Should().Equal(DefaultUserGuid);

        var capturedMessages = new List<TaskActionMessage>();
        this.MessageBrokerMock.Verify(messageBroker => messageBroker
            .Publish(Capture.In(capturedMessages), It.IsAny<CancellationToken>()));
        capturedMessages.Should().SatisfyRespectively(message =>
        {
            message.ActorUserId.Should().Be(DefaultAdminGuid);
            message.Action.Should().Be(TaskAction.Reopen);
            message.Task.Should().BeEquivalentTo(reopenedTask);
            message.Project.Should().BeEquivalentTo(project);
        });

        var taskDetails = await this.TaskClient.GetTaskDetailsAsync(task.Id);

        taskDetails.Should().Be(reopenedTask);
    }

    [Fact]
    public async Task ReopenTaskAsManager()
    {
        this.ActAsAdmin();

        var project = await this.ProjectClient.CreateProjectAsync(new ProjectSaveRequest
        {
            DisplayName = "ProjectDisplayName",
            Members = new Dictionary<Guid, ProjectMemberRole> { { DefaultUserGuid, ProjectMemberRole.Manager } }
                .ToImmutableDictionary()
        });

        var task = await this.TaskClient.CreateTaskAsync(new TaskCreateRequest
        {
            ProjectId = project.Id,
            DisplayName = "DisplayName",
            Description = "Description",
            Assignees = []
        });

        this.ActAsUser();

        await this.TaskClient.CloseTaskAsync(task.Id);

        this.MessageBrokerMock.Reset();

        var reopenedTask = await this.TaskClient.ReopenTaskAsync(task.Id);

        reopenedTask.Id.Should().Be(task.Id);
        reopenedTask.DisplayName.Should().Be("DisplayName");
        reopenedTask.Description.Should().Be("Description");
        reopenedTask.Open.Should().BeTrue();
        reopenedTask.Assignees.Should().Equal();

        var capturedMessages = new List<TaskActionMessage>();
        this.MessageBrokerMock.Verify(messageBroker => messageBroker
            .Publish(Capture.In(capturedMessages), It.IsAny<CancellationToken>()));
        capturedMessages.Should().SatisfyRespectively(message =>
        {
            message.ActorUserId.Should().Be(DefaultUserGuid);
            message.Action.Should().Be(TaskAction.Reopen);
            message.Task.Should().BeEquivalentTo(reopenedTask);
            message.Project.Should().BeEquivalentTo(project);
        });

        var taskDetails = await this.TaskClient.GetTaskDetailsAsync(task.Id);

        taskDetails.Should().Be(reopenedTask);
    }

    [Fact]
    public async Task ReopenTaskAsAssignee()
    {
        this.ActAsAdmin();

        var project = await this.ProjectClient.CreateProjectAsync(new ProjectSaveRequest
        {
            DisplayName = "ProjectDisplayName",
            Members = ImmutableDictionary<Guid, ProjectMemberRole>.Empty
        });

        var task = await this.TaskClient.CreateTaskAsync(new TaskCreateRequest
        {
            ProjectId = project.Id,
            DisplayName = "DisplayName",
            Description = "Description",
            Assignees = [DefaultUserGuid]
        });

        this.ActAsUser();

        await this.TaskClient.CloseTaskAsync(task.Id);

        var reopenedTask = await this.TaskClient.ReopenTaskAsync(task.Id);

        reopenedTask.Id.Should().Be(task.Id);
        reopenedTask.DisplayName.Should().Be("DisplayName");
        reopenedTask.Description.Should().Be("Description");
        reopenedTask.Open.Should().BeTrue();
        reopenedTask.Assignees.Should().Equal(DefaultUserGuid);
    }

    [Fact]
    public async Task ReopenTaskIsForbiddenWhenUserIsNotAssigned()
    {
        this.ActAsAdmin();

        var project = await this.ProjectClient.CreateProjectAsync(new ProjectSaveRequest
        {
            DisplayName = "ProjectDisplayName",
            Members = new Dictionary<Guid, ProjectMemberRole> { { DefaultUserGuid, ProjectMemberRole.Worker } }
                .ToImmutableDictionary()
        });

        var task = await this.TaskClient.CreateTaskAsync(new TaskCreateRequest
        {
            ProjectId = project.Id,
            DisplayName = "DisplayName",
            Description = "Description",
            Assignees = []
        });

        await this.TaskClient.CloseTaskAsync(task.Id);

        this.ActAsUser();

        await Invoking(() => this.TaskClient.ReopenTaskAsync(task.Id))
            .Should().ThrowAsync<HttpRequestException>()
            .Where(exception => exception.StatusCode == HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task ReopenTaskIsForbiddenWhenTaskIsAlreadyOpen()
    {
        this.ActAsAdmin();

        var project = await this.ProjectClient.CreateProjectAsync(new ProjectSaveRequest
        {
            DisplayName = "ProjectDisplayName",
            Members = ImmutableDictionary<Guid, ProjectMemberRole>.Empty
        });

        var task = await this.TaskClient.CreateTaskAsync(new TaskCreateRequest
        {
            ProjectId = project.Id,
            DisplayName = "DisplayName",
            Description = "Description",
            Assignees = []
        });

        await Invoking(() => this.TaskClient.ReopenTaskAsync(task.Id))
            .Should().ThrowAsync<HttpRequestException>()
            .Where(exception => exception.StatusCode == HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task ReopenTaskNotPossibleWhenProjectIsArchived()
    {
        this.ActAsAdmin();

        var project = await this.ProjectClient.CreateProjectAsync(new ProjectSaveRequest
        {
            DisplayName = "ProjectDisplayName",
            Members = ImmutableDictionary<Guid, ProjectMemberRole>.Empty
        });

        var task = await this.TaskClient.CreateTaskAsync(new TaskCreateRequest
        {
            ProjectId = project.Id,
            DisplayName = "DisplayName",
            Description = "Description",
            Assignees = []
        });

        await this.TaskClient.CloseTaskAsync(task.Id);

        await this.ProjectClient.ArchiveProjectAsync(project.Id);

        await Invoking(() => this.TaskClient.CloseTaskAsync(task.Id))
            .Should().ThrowAsync<HttpRequestException>()
            .Where(exception => exception.StatusCode == HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetTaskHistoryAsAdministrator()
    {
        this.ActAsAdmin();

        var project = await this.ProjectClient.CreateProjectAsync(new ProjectSaveRequest
        {
            DisplayName = "ProjectDisplayName",
            Members = ImmutableDictionary<Guid, ProjectMemberRole>.Empty
        });

        var createdTask = await this.TaskClient.CreateTaskAsync(new TaskCreateRequest
        {
            ProjectId = project.Id,
            DisplayName = "DisplayNameA",
            Description = "DescriptionA",
            Assignees = [DefaultUserGuid]
        });

        var updatedTask = await this.TaskClient.UpdateTaskAsync(
            createdTask.Id,
            new TaskUpdateRequest
            {
                DisplayName = "DisplayNameB",
                Description = "DescriptionB",
                Assignees = [DefaultUserGuid]
            });

        var closedTask = await this.TaskClient.CloseTaskAsync(createdTask.Id);

        var reopenedTask = await this.TaskClient.ReopenTaskAsync(createdTask.Id);

        var history = await this.TaskClient.GetTaskHistoryAsync(createdTask.Id);

        history.Should().SatisfyRespectively(
            entry =>
            {
                entry.Action.Should().Be(TaskAction.Create);
                entry.Entity.Should().Be(createdTask);
                entry.User.Id.Should().Be(DefaultAdminGuid);
                entry.User.FirstName.Should().Be("FirstNameAdmin");
                entry.User.LastName.Should().Be("LastNameAdmin");
            },
            entry =>
            {
                entry.Action.Should().Be(TaskAction.Update);
                entry.Entity.Should().Be(updatedTask);
                entry.User.Id.Should().Be(DefaultAdminGuid);
                entry.User.FirstName.Should().Be("FirstNameAdmin");
                entry.User.LastName.Should().Be("LastNameAdmin");
            },
            entry =>
            {
                entry.Action.Should().Be(TaskAction.Close);
                entry.Entity.Should().Be(closedTask);
                entry.User.Id.Should().Be(DefaultAdminGuid);
                entry.User.FirstName.Should().Be("FirstNameAdmin");
                entry.User.LastName.Should().Be("LastNameAdmin");
            },
            entry =>
            {
                entry.Action.Should().Be(TaskAction.Reopen);
                entry.Entity.Should().Be(reopenedTask);
                entry.User.Id.Should().Be(DefaultAdminGuid);
                entry.User.FirstName.Should().Be("FirstNameAdmin");
                entry.User.LastName.Should().Be("LastNameAdmin");
            }
        );
    }

    [Fact]
    public async Task GetTaskHistoryAsNormalUserWithProjectMembership()
    {
        this.ActAsAdmin();

        var project = await this.ProjectClient.CreateProjectAsync(new ProjectSaveRequest
        {
            DisplayName = "ProjectDisplayName",
            Members = new Dictionary<Guid, ProjectMemberRole> { { DefaultUserGuid, ProjectMemberRole.Manager } }
                .ToImmutableDictionary()
        });

        var createdTask = await this.TaskClient.CreateTaskAsync(new TaskCreateRequest
        {
            ProjectId = project.Id,
            DisplayName = "DisplayNameA",
            Description = "DescriptionA",
            Assignees = []
        });

        var updatedTask = await this.TaskClient.UpdateTaskAsync(
            createdTask.Id,
            new TaskUpdateRequest
            {
                DisplayName = "DisplayNameB",
                Description = "DescriptionB",
                Assignees = []
            });

        var closedTask = await this.TaskClient.CloseTaskAsync(createdTask.Id);

        var reopenedTask = await this.TaskClient.ReopenTaskAsync(createdTask.Id);

        this.ActAsUser();

        var history = await this.TaskClient.GetTaskHistoryAsync(createdTask.Id);

        history.Should().SatisfyRespectively(
            entry =>
            {
                entry.Action.Should().Be(TaskAction.Create);
                entry.Entity.Should().Be(createdTask);
                entry.User.Id.Should().Be(DefaultAdminGuid);
                entry.User.FirstName.Should().Be("FirstNameAdmin");
                entry.User.LastName.Should().Be("LastNameAdmin");
            },
            entry =>
            {
                entry.Action.Should().Be(TaskAction.Update);
                entry.Entity.Should().Be(updatedTask);
                entry.User.Id.Should().Be(DefaultAdminGuid);
                entry.User.FirstName.Should().Be("FirstNameAdmin");
                entry.User.LastName.Should().Be("LastNameAdmin");
            },
            entry =>
            {
                entry.Action.Should().Be(TaskAction.Close);
                entry.Entity.Should().Be(closedTask);
                entry.User.Id.Should().Be(DefaultAdminGuid);
                entry.User.FirstName.Should().Be("FirstNameAdmin");
                entry.User.LastName.Should().Be("LastNameAdmin");
            },
            entry =>
            {
                entry.Action.Should().Be(TaskAction.Reopen);
                entry.Entity.Should().Be(reopenedTask);
                entry.User.Id.Should().Be(DefaultAdminGuid);
                entry.User.FirstName.Should().Be("FirstNameAdmin");
                entry.User.LastName.Should().Be("LastNameAdmin");
            }
        );
    }

    [Fact]
    public async Task GetTaskHistoryWithArchivedProject()
    {
        this.ActAsAdmin();

        var project = await this.ProjectClient.CreateProjectAsync(new ProjectSaveRequest
        {
            DisplayName = "ProjectDisplayName",
            Members = new Dictionary<Guid, ProjectMemberRole> { { DefaultUserGuid, ProjectMemberRole.Manager } }
                .ToImmutableDictionary()
        });

        var createdTask = await this.TaskClient.CreateTaskAsync(new TaskCreateRequest
        {
            ProjectId = project.Id,
            DisplayName = "DisplayName",
            Description = "Description",
            Assignees = []
        });

        await this.ProjectClient.ArchiveProjectAsync(project.Id);

        this.ActAsUser();

        var history = await this.TaskClient.GetTaskHistoryAsync(createdTask.Id);

        history.Should().SatisfyRespectively(entry =>
            {
                entry.Action.Should().Be(TaskAction.Create);
                entry.Entity.Should().Be(createdTask);
                entry.User.Id.Should().Be(DefaultAdminGuid);
                entry.User.FirstName.Should().Be("FirstNameAdmin");
                entry.User.LastName.Should().Be("LastNameAdmin");
            }
        );
    }

    [Fact]
    public async Task GetTaskHistoryIsForbiddenWhenUserIsNotProjectMember()
    {
        this.ActAsAdmin();

        var project = await this.ProjectClient.CreateProjectAsync(new ProjectSaveRequest
        {
            DisplayName = "ProjectDisplayName",
            Members = ImmutableDictionary<Guid, ProjectMemberRole>.Empty
        });

        var task = await this.TaskClient.CreateTaskAsync(new TaskCreateRequest
        {
            ProjectId = project.Id,
            DisplayName = "DisplayName",
            Description = "Description",
            Assignees = []
        });

        this.ActAsUser();

        await Invoking(() => this.TaskClient.GetTaskHistoryAsync(task.Id))
            .Should().ThrowAsync<HttpRequestException>()
            .Where(exception => exception.StatusCode == HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetTaskHistoryWithUnknownTaskId()
    {
        this.ActAsAdmin();

        await Invoking(() => this.TaskClient.GetTaskHistoryAsync(Guid.Empty))
            .Should().ThrowAsync<HttpRequestException>()
            .Where(exception => exception.StatusCode == HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task ListTasksForProject()
    {
        this.ActAsAdmin();

        var project = await this.ProjectClient.CreateProjectAsync(new ProjectSaveRequest
        {
            DisplayName = "ProjectDisplayName",
            Members = ImmutableDictionary<Guid, ProjectMemberRole>.Empty
        });

        var taskA = await this.TaskClient.CreateTaskAsync(new TaskCreateRequest
        {
            ProjectId = project.Id,
            DisplayName = "DisplayNameA",
            Description = "Description",
            Assignees = [DefaultUserGuid]
        });
        var taskB = await this.TaskClient.CreateTaskAsync(new TaskCreateRequest
        {
            ProjectId = project.Id,
            DisplayName = "DisplayNameA",
            Description = "Description",
            Assignees = [DefaultUserGuid]
        });
        var taskC = await this.TaskClient.CreateTaskAsync(new TaskCreateRequest
        {
            ProjectId = project.Id,
            DisplayName = "DisplayNameA",
            Description = "Description",
            Assignees = [DefaultUserGuid]
        });

        var page1 = await this.TaskClient.ListTasksForProjectAsync(project.Id, 0, 2);

        page1.Number.Should().Be(0);
        page1.Size.Should().Be(2);
        page1.TotalElements.Should().Be(3);
        page1.TotalPages.Should().Be(2);
        page1.Content.Should().Equal(taskA, taskB);

        var page2 = await this.TaskClient.ListTasksForProjectAsync(project.Id, 1, 2);

        page2.Number.Should().Be(1);
        page2.Size.Should().Be(2);
        page2.TotalElements.Should().Be(3);
        page2.TotalPages.Should().Be(2);
        page2.Content.Should().Equal(taskC);
    }

    [Fact]
    public async Task ListTasksForProjectIsForbiddenWhenUserIsNotProjectMember()
    {
        this.ActAsAdmin();

        var project = await this.ProjectClient.CreateProjectAsync(new ProjectSaveRequest
        {
            DisplayName = "ProjectDisplayName",
            Members = ImmutableDictionary<Guid, ProjectMemberRole>.Empty
        });

        this.ActAsUser();

        await Invoking(() => this.TaskClient.ListTasksForProjectAsync(project.Id, 0, 2))
            .Should().ThrowAsync<HttpRequestException>()
            .Where(exception => exception.StatusCode == HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task ListTasksForProjectWithInvalidProjectId()
    {
        this.ActAsAdmin();

        await Invoking(() => this.TaskClient.ListTasksForProjectAsync(Guid.Empty, 0, 2))
            .Should().ThrowAsync<HttpRequestException>()
            .Where(exception => exception.StatusCode == HttpStatusCode.NotFound);
    }
}
