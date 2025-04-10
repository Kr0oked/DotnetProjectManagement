namespace DotnetProjectManagement.ProjectManagement.App.APIs;

using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using UseCases.DTOs;
using UseCases.Exceptions;
using UseCases.Project.Archive;
using UseCases.Project.Create;
using UseCases.Project.GetDetails;
using UseCases.Project.List;
using UseCases.Project.Restore;
using UseCases.Project.Update;
using Web.Models;

public static class ProjectApi
{
    public static RouteGroupBuilder MapProjectApi(this IEndpointRouteBuilder app)
    {
        var api = app.MapGroup("projects")
            .WithTags("Projects");

        api.MapGet("/", ListProjectsAsync)
            .WithName("ListProjects")
            .WithSummary("List projects")
            .WithDescription("Get a paginated list of projects.");
        api.MapPost("/", CreateProjectAsync)
            .WithName("CreateProject")
            .WithSummary("Create project")
            .WithDescription("Create a new project.");
        api.MapGet("/{projectId:guid}", GetProjectDetailsAsync)
            .WithName("GetProjectDetails")
            .WithSummary("Get project")
            .WithDescription("Get details for a project.");
        api.MapPut("/{projectId:guid}", UpdateProjectAsync)
            .WithName("UpdateProject")
            .WithSummary("Update project")
            .WithDescription("Update a project.");
        api.MapPost("/{projectId:guid}/archive", ArchiveProjectAsync)
            .WithName("ArchiveProject")
            .WithSummary("Archive project")
            .WithDescription("Archive an active project.");
        api.MapPost("/{projectId:guid}/restore", RestoreProjectAsync)
            .WithName("RestoreProject")
            .WithSummary("Restore project")
            .WithDescription("Restore an archived project.");

        return api;
    }

    private static async Task<Ok<PageRepresentation<ProjectRepresentation>>> ListProjectsAsync(
        ClaimsPrincipal user,
        [FromServices] ProjectListUseCase useCase,
        [FromQuery, Range(0, int.MaxValue)] int pageNumber = 0,
        [FromQuery, Range(1, 100)] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var actor = user.ToActor();
        var pageRequest = new PageRequest(pageNumber, pageSize);
        var page = await useCase.ListProjectsAsync(actor, pageRequest, cancellationToken);
        return TypedResults.Ok(page.ToWeb());
    }

    private static async
        Task<Results<
            Created<ProjectRepresentation>,
            BadRequest<ProblemDetails>,
            UnauthorizedHttpResult,
            ValidationProblem>>
        CreateProjectAsync(
            ClaimsPrincipal user,
            [FromServices] ProjectCreateUseCase useCase,
            [FromBody] ProjectSaveRequest body,
            CancellationToken cancellationToken = default)
    {
        var validationResults = new List<ValidationResult>();
        var isValid = Validator.TryValidateObject(body, new ValidationContext(body), validationResults, true);
        if (!isValid)
        {
            return TypedResults.ValidationProblem(validationResults.ToErrorDictionary());
        }

        try
        {
            var actor = user.ToActor();
            var command = new ProjectCreateCommand
            {
                DisplayName = body.DisplayName,
                Members = body.Members
            };
            var project = await useCase.CreateProjectAsync(actor, command, cancellationToken);
            return TypedResults.Created($"api/projects/{project.Id}", project.ToWeb());
        }
        catch (UserNotFoundException exception)
        {
            return TypedResults.BadRequest(new ProblemDetails { Detail = exception.Message });
        }
        catch (AdministratorRequiredException)
        {
            return TypedResults.Unauthorized();
        }
    }

    private static async Task<Results<Ok<ProjectRepresentation>, UnauthorizedHttpResult, NotFound<ProblemDetails>>>
        GetProjectDetailsAsync(
            ClaimsPrincipal user,
            [FromServices] ProjectGetDetailsUseCase useCase,
            [FromRoute] Guid projectId,
            CancellationToken cancellationToken = default)
    {
        try
        {
            var actor = user.ToActor();
            var project = await useCase.GetProjectDetailsAsync(actor, projectId, cancellationToken);
            return TypedResults.Ok(project.ToWeb());
        }
        catch (ProjectMemberException)
        {
            return TypedResults.Unauthorized();
        }
        catch (ProjectNotFoundException exception)
        {
            return TypedResults.NotFound(new ProblemDetails { Detail = exception.Message });
        }
    }

    private static async Task<Results<
            Ok<ProjectRepresentation>,
            BadRequest<ProblemDetails>,
            UnauthorizedHttpResult,
            NotFound<ProblemDetails>,
            ValidationProblem>>
        UpdateProjectAsync(
            ClaimsPrincipal user,
            [FromServices] ProjectUpdateUseCase useCase,
            [FromRoute] Guid projectId,
            [FromBody] ProjectSaveRequest body,
            CancellationToken cancellationToken = default)
    {
        var validationResults = new List<ValidationResult>();
        var isValid = Validator.TryValidateObject(body, new ValidationContext(body), validationResults, true);
        if (!isValid)
        {
            return TypedResults.ValidationProblem(validationResults.ToErrorDictionary());
        }

        try
        {
            var actor = user.ToActor();

            var command = new ProjectUpdateCommand
            {
                ProjectId = projectId,
                DisplayName = body.DisplayName,
                Members = body.Members
            };

            var project = await useCase.UpdateProjectAsync(actor, command, cancellationToken);
            return TypedResults.Ok(project.ToWeb());
        }
        catch (UserNotFoundException exception)
        {
            return TypedResults.BadRequest(new ProblemDetails { Detail = exception.Message });
        }
        catch (ProjectArchivedException exception)
        {
            return TypedResults.BadRequest(new ProblemDetails { Detail = exception.Message });
        }
        catch (ManagerRequiredException)
        {
            return TypedResults.Unauthorized();
        }
        catch (ProjectNotFoundException exception)
        {
            return TypedResults.NotFound(new ProblemDetails { Detail = exception.Message });
        }
    }

    private static async Task<Results<
            Ok<ProjectRepresentation>,
            BadRequest<ProblemDetails>,
            UnauthorizedHttpResult,
            NotFound<ProblemDetails>>>
        ArchiveProjectAsync(
            ClaimsPrincipal user,
            [FromServices] ProjectArchiveUseCase projectArchiveUseCase,
            [FromRoute] Guid projectId,
            CancellationToken cancellationToken = default)
    {
        try
        {
            var actor = user.ToActor();
            var project = await projectArchiveUseCase.ArchiveProjectAsync(actor, projectId, cancellationToken);
            return TypedResults.Ok(project.ToWeb());
        }
        catch (ProjectAlreadyArchivedException exception)
        {
            return TypedResults.BadRequest(new ProblemDetails { Detail = exception.Message });
        }
        catch (ManagerRequiredException)
        {
            return TypedResults.Unauthorized();
        }
        catch (ProjectNotFoundException exception)
        {
            return TypedResults.NotFound(new ProblemDetails { Detail = exception.Message });
        }
    }

    private static async Task<Results<
            Ok<ProjectRepresentation>,
            BadRequest<ProblemDetails>,
            UnauthorizedHttpResult,
            NotFound<ProblemDetails>>>
        RestoreProjectAsync(
            ClaimsPrincipal user,
            [FromServices] ProjectRestoreUseCase useCase,
            [FromRoute] Guid projectId,
            CancellationToken cancellationToken = default)
    {
        try
        {
            var actor = user.ToActor();
            var project = await useCase.RestoreProjectAsync(actor, projectId, cancellationToken);
            return TypedResults.Ok(project.ToWeb());
        }
        catch (ProjectNotArchivedException exception)
        {
            return TypedResults.BadRequest(new ProblemDetails { Detail = exception.Message });
        }
        catch (ManagerRequiredException)
        {
            return TypedResults.Unauthorized();
        }
        catch (ProjectNotFoundException exception)
        {
            return TypedResults.NotFound(new ProblemDetails { Detail = exception.Message });
        }
    }

    private static PageRepresentation<ProjectRepresentation> ToWeb(this Page<ProjectDto> page) =>
        new()
        {
            Size = page.Size,
            TotalElements = page.TotalElements,
            TotalPages = page.TotalPages,
            Number = page.Number,
            Content = page.Content
                .Select(project => project.ToWeb())
                .ToImmutableList()
        };

    private static ProjectRepresentation ToWeb(this ProjectDto project) =>
        new()
        {
            Id = project.Id,
            DisplayName = project.DisplayName,
            Archived = project.Archived,
            Members = project.Members
        };
}
