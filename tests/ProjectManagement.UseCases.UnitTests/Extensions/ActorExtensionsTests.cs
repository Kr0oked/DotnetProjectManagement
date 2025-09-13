namespace DotnetProjectManagement.ProjectManagement.UseCases.UnitTests.Extensions;

using Domain.Entities;
using Exceptions;
using FluentAssertions;
using UseCases.DTOs;
using UseCases.Extensions;
using Xunit;
using static FluentAssertions.FluentActions;

public class ActorExtensionsTests
{
    [Fact]
    public void VerifyIsAdministratorDoesNotThrowWhenActorIsAdministrator()
    {
        var actor = new Actor
        {
            UserId = new Guid("fd9f45e1-48f0-42ae-a390-2f4d1653451f"),
            FirstName = "FirstName",
            LastName = "Lastname",
            IsAdministrator = true
        };

        Invoking(actor.VerifyIsAdministrator)
            .Should().NotThrow();
    }

    [Fact]
    public void VerifyIsAdministratorThrowsAdministratorRequiredExceptionWhenActorIsNotAdministrator()
    {
        var userId = new Guid("fd9f45e1-48f0-42ae-a390-2f4d1653451f");
        var actor = new Actor
        {
            UserId = userId,
            FirstName = "FirstName",
            LastName = "Lastname",
            IsAdministrator = false
        };

        Invoking(actor.VerifyIsAdministrator)
            .Should().Throw<AdministratorRequiredException>()
            .WithMessage($"User {userId} is not an administrator");
    }

    [Fact]
    public void VerifyIsManagerDoesNotThrowWhenActorIsAdministrator()
    {
        var actor = new Actor
        {
            UserId = new Guid("fd9f45e1-48f0-42ae-a390-2f4d1653451f"),
            FirstName = "FirstName",
            LastName = "Lastname",
            IsAdministrator = true
        };
        var project = new Project
        {
            Id = new Guid("00aaba25-ffad-4564-bffc-57a3b85fc171"),
            DisplayName = "DisplayName",
            Archived = false,
            Members = []
        };

        Invoking(() => actor.VerifyIsManager(project))
            .Should().NotThrow();
    }

    [Fact]
    public void VerifyIsManagerDoesNotThrowWhenActorIsManager()
    {
        var userId = new Guid("fd9f45e1-48f0-42ae-a390-2f4d1653451f");
        var actor = new Actor
        {
            UserId = userId,
            FirstName = "FirstName",
            LastName = "Lastname",
            IsAdministrator = false
        };
        var project = new Project
        {
            Id = new Guid("00aaba25-ffad-4564-bffc-57a3b85fc171"),
            DisplayName = "DisplayName",
            Archived = false,
            Members = new Dictionary<Guid, ProjectMemberRole> { { userId, ProjectMemberRole.Manager } }
        };

        Invoking(() => actor.VerifyIsManager(project))
            .Should().NotThrow();
    }

    [Fact]
    public void VerifyIsManagerThrowsManagerRequiredExceptionWhenActorIsWorker()
    {
        var userId = new Guid("fd9f45e1-48f0-42ae-a390-2f4d1653451f");
        var projectId = new Guid("00aaba25-ffad-4564-bffc-57a3b85fc171");
        var actor = new Actor
        {
            UserId = userId,
            FirstName = "FirstName",
            LastName = "Lastname",
            IsAdministrator = false
        };
        var project = new Project
        {
            Id = projectId,
            DisplayName = "DisplayName",
            Archived = false,
            Members = new Dictionary<Guid, ProjectMemberRole> { { userId, ProjectMemberRole.Worker } }
        };

        Invoking(() => actor.VerifyIsManager(project))
            .Should().Throw<ManagerRequiredException>()
            .WithMessage($"User {userId} is not a manager in project {projectId}");
    }

    [Fact]
    public void VerifyIsManagerThrowsManagerRequiredExceptionWhenActorIsGuest()
    {
        var userId = new Guid("fd9f45e1-48f0-42ae-a390-2f4d1653451f");
        var projectId = new Guid("00aaba25-ffad-4564-bffc-57a3b85fc171");
        var actor = new Actor
        {
            UserId = userId,
            FirstName = "FirstName",
            LastName = "Lastname",
            IsAdministrator = false
        };
        var project = new Project
        {
            Id = projectId,
            DisplayName = "DisplayName",
            Archived = false,
            Members = new Dictionary<Guid, ProjectMemberRole> { { userId, ProjectMemberRole.Guest } }
        };

        Invoking(() => actor.VerifyIsManager(project))
            .Should().Throw<ManagerRequiredException>()
            .WithMessage($"User {userId} is not a manager in project {projectId}");
    }

    [Fact]
    public void VerifyIsManagerThrowsManagerRequiredExceptionWhenActorIsNotProjectMember()
    {
        var userId = new Guid("fd9f45e1-48f0-42ae-a390-2f4d1653451f");
        var projectId = new Guid("00aaba25-ffad-4564-bffc-57a3b85fc171");
        var actor = new Actor
        {
            UserId = userId,
            FirstName = "FirstName",
            LastName = "Lastname",
            IsAdministrator = false
        };
        var project = new Project
        {
            Id = projectId,
            DisplayName = "DisplayName",
            Archived = false,
            Members = []
        };

        Invoking(() => actor.VerifyIsManager(project))
            .Should().Throw<ManagerRequiredException>()
            .WithMessage($"User {userId} is not a manager in project {projectId}");
    }

    [Fact]
    public void VerifyIsProjectMemberDoesNotThrowWhenActorIsAdministrator()
    {
        var actor = new Actor
        {
            UserId = new Guid("fd9f45e1-48f0-42ae-a390-2f4d1653451f"),
            FirstName = "FirstName",
            LastName = "Lastname",
            IsAdministrator = true
        };
        var project = new Project
        {
            Id = new Guid("00aaba25-ffad-4564-bffc-57a3b85fc171"),
            DisplayName = "DisplayName",
            Archived = false,
            Members = []
        };

        Invoking(() => actor.VerifyIsProjectMember(project))
            .Should().NotThrow();
    }

    [Fact]
    public void VerifyIsProjectMemberDoesNotThrowWhenActorIsManager()
    {
        var userId = new Guid("fd9f45e1-48f0-42ae-a390-2f4d1653451f");
        var actor = new Actor
        {
            UserId = userId,
            FirstName = "FirstName",
            LastName = "Lastname",
            IsAdministrator = false
        };
        var project = new Project
        {
            Id = new Guid("00aaba25-ffad-4564-bffc-57a3b85fc171"),
            DisplayName = "DisplayName",
            Archived = false,
            Members = new Dictionary<Guid, ProjectMemberRole> { { userId, ProjectMemberRole.Manager } }
        };

        Invoking(() => actor.VerifyIsProjectMember(project))
            .Should().NotThrow();
    }

    [Fact]
    public void VerifyIsProjectMemberDoesNotThrowWhenActorIsWorker()
    {
        var userId = new Guid("fd9f45e1-48f0-42ae-a390-2f4d1653451f");
        var actor = new Actor
        {
            UserId = userId,
            FirstName = "FirstName",
            LastName = "Lastname",
            IsAdministrator = false
        };
        var project = new Project
        {
            Id = new Guid("00aaba25-ffad-4564-bffc-57a3b85fc171"),
            DisplayName = "DisplayName",
            Archived = false,
            Members = new Dictionary<Guid, ProjectMemberRole> { { userId, ProjectMemberRole.Worker } }
        };

        Invoking(() => actor.VerifyIsProjectMember(project))
            .Should().NotThrow();
    }

    [Fact]
    public void VerifyIsProjectMemberDoesNotThrowWhenActorIsGuest()
    {
        var userId = new Guid("fd9f45e1-48f0-42ae-a390-2f4d1653451f");
        var actor = new Actor
        {
            UserId = userId,
            FirstName = "FirstName",
            LastName = "Lastname",
            IsAdministrator = false
        };
        var project = new Project
        {
            Id = new Guid("00aaba25-ffad-4564-bffc-57a3b85fc171"),
            DisplayName = "DisplayName",
            Archived = false,
            Members = new Dictionary<Guid, ProjectMemberRole> { { userId, ProjectMemberRole.Guest } }
        };

        Invoking(() => actor.VerifyIsProjectMember(project))
            .Should().NotThrow();
    }

    [Fact]
    public void VerifyIsProjectMemberThrowsManagerRequiredExceptionWhenActorIsNotProjectMember()
    {
        var userId = new Guid("fd9f45e1-48f0-42ae-a390-2f4d1653451f");
        var projectId = new Guid("00aaba25-ffad-4564-bffc-57a3b85fc171");
        var actor = new Actor
        {
            UserId = userId,
            FirstName = "FirstName",
            LastName = "Lastname",
            IsAdministrator = false
        };
        var project = new Project
        {
            Id = projectId,
            DisplayName = "DisplayName",
            Archived = false,
            Members = []
        };

        Invoking(() => actor.VerifyIsProjectMember(project))
            .Should().Throw<ProjectMemberException>()
            .WithMessage($"User {userId} is not a member of project {projectId}");
    }
}
