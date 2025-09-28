namespace DotnetProjectManagement.ProjectManagement.App.APIs;

using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Domain.Actions;
using Extensions;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using UseCases.DTOs;
using UseCases.Exceptions;
using UseCases.ProjectTask.Close;
using UseCases.ProjectTask.Create;
using UseCases.ProjectTask.GetDetails;
using UseCases.ProjectTask.GetHistory;
using UseCases.ProjectTask.ListForProject;
using UseCases.ProjectTask.Reopen;
using UseCases.ProjectTask.Update;
using Web.Models;

public static class TaskApi
{
    public static RouteGroupBuilder MapTaskApi(this IEndpointRouteBuilder app)
    {
        var api = app.MapGroup("tasks")
            .WithTags("Tasks");

        api.MapPost("/", CreateTaskAsync)
            .WithName("CreateTask")
            .WithSummary("Create task")
            .WithDescription("Create a new task.");
        api.MapGet("/{taskId:guid}", GetTaskDetailsAsync)
            .WithName("GetTaskDetails")
            .WithSummary("Get task")
            .WithDescription("Get details for a task.");
        api.MapPut("/{taskId:guid}", UpdateTaskAsync)
            .WithName("UpdateTask")
            .WithSummary("Update task")
            .WithDescription("Update a task.");
        api.MapPost("/{taskId:guid}/close", CloseTaskAsync)
            .WithName("CloseTask")
            .WithSummary("Close task")
            .WithDescription("Closes a task.");
        api.MapPost("/{taskId:guid}/reopen", ReopenTaskAsync)
            .WithName("ReopenTask")
            .WithSummary("Reopen task")
            .WithDescription("Reopens a task.");
        api.MapGet("/{taskId:guid}/history", GetTaskHistoryAsync)
            .WithName("GetTaskHistory")
            .WithSummary("Get task history")
            .WithDescription("Get history for a task.");
        api.MapGet("/project/{projectId:guid}", ListTasksForProjectAsync)
            .WithName("ListTasksForProject")
            .WithSummary("List tasks for a project")
            .WithDescription("Get a paginated list of tasks for a project.");

        return api;
    }

    private static async
        Task<Results<
            Created<TaskRepresentation>,
            BadRequest<ProblemDetails>,
            UnauthorizedHttpResult,
            ValidationProblem>>
        CreateTaskAsync(
            ClaimsPrincipal principal,
            [FromServices] TaskCreateUseCase useCase,
            [FromBody] TaskCreateRequest body,
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
            var actor = principal.ToActor();
            var command = new TaskCreateCommand
            {
                ProjectId = body.ProjectId,
                DisplayName = body.DisplayName,
                Description = body.Description,
                Assignees = body.Assignees
            };
            var task = await useCase.CreateTaskAsync(actor, command, cancellationToken);
            return TypedResults.Created($"tasks/{task.Id}", task.ToWeb());
        }
        catch (ProjectArchivedException exception)
        {
            return TypedResults.BadRequest(new ProblemDetails { Detail = exception.Message });
        }
        catch (UserNotFoundException exception)
        {
            return TypedResults.BadRequest(new ProblemDetails { Detail = exception.Message });
        }
        catch (ManagerRequiredException)
        {
            return TypedResults.Unauthorized();
        }
    }

    private static async Task<Results<Ok<TaskRepresentation>, UnauthorizedHttpResult, NotFound<ProblemDetails>>>
        GetTaskDetailsAsync(
            ClaimsPrincipal principal,
            [FromServices] TaskGetDetailsUseCase useCase,
            [FromRoute] Guid taskId,
            CancellationToken cancellationToken = default)
    {
        try
        {
            var actor = principal.ToActor();
            var task = await useCase.GetTaskDetailsAsync(actor, taskId, cancellationToken);
            return TypedResults.Ok(task.ToWeb());
        }
        catch (ProjectMemberException)
        {
            return TypedResults.Unauthorized();
        }
        catch (ProjectNotFoundException exception)
        {
            return TypedResults.NotFound(new ProblemDetails { Detail = exception.Message });
        }
        catch (TaskNotFoundException exception)
        {
            return TypedResults.NotFound(new ProblemDetails { Detail = exception.Message });
        }
    }

    private static async Task<Results<
            Ok<TaskRepresentation>,
            BadRequest<ProblemDetails>,
            UnauthorizedHttpResult,
            NotFound<ProblemDetails>,
            ValidationProblem>>
        UpdateTaskAsync(
            ClaimsPrincipal principal,
            [FromServices] TaskUpdateUseCase useCase,
            [FromRoute] Guid taskId,
            [FromBody] TaskUpdateRequest body,
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
            var actor = principal.ToActor();

            var command = new TaskUpdateCommand
            {
                TaskId = taskId,
                DisplayName = body.DisplayName,
                Description = body.Description,
                Assignees = body.Assignees
            };

            var task = await useCase.UpdateTaskAsync(actor, command, cancellationToken);
            return TypedResults.Ok(task.ToWeb());
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
        catch (TaskNotFoundException exception)
        {
            return TypedResults.NotFound(new ProblemDetails { Detail = exception.Message });
        }
    }

    private static async Task<Results<
            Ok<TaskRepresentation>,
            BadRequest<ProblemDetails>,
            UnauthorizedHttpResult,
            NotFound<ProblemDetails>>>
        CloseTaskAsync(
            ClaimsPrincipal principal,
            [FromServices] TaskCloseUseCase taskCloseUseCase,
            [FromRoute] Guid taskId,
            CancellationToken cancellationToken = default)
    {
        try
        {
            var actor = principal.ToActor();
            var project = await taskCloseUseCase.CloseTaskAsync(actor, taskId, cancellationToken);
            return TypedResults.Ok(project.ToWeb());
        }
        catch (ProjectArchivedException exception)
        {
            return TypedResults.BadRequest(new ProblemDetails { Detail = exception.Message });
        }
        catch (TaskClosedException exception)
        {
            return TypedResults.BadRequest(new ProblemDetails { Detail = exception.Message });
        }
        catch (ManagerRequiredException)
        {
            return TypedResults.Unauthorized();
        }
        catch (TaskNotFoundException exception)
        {
            return TypedResults.NotFound(new ProblemDetails { Detail = exception.Message });
        }
    }

    private static async Task<Results<
            Ok<TaskRepresentation>,
            BadRequest<ProblemDetails>,
            UnauthorizedHttpResult,
            NotFound<ProblemDetails>>>
        ReopenTaskAsync(
            ClaimsPrincipal principal,
            [FromServices] TaskReopenUseCase taskReopenUseCase,
            [FromRoute] Guid taskId,
            CancellationToken cancellationToken = default)
    {
        try
        {
            var actor = principal.ToActor();
            var project = await taskReopenUseCase.ReopenTaskAsync(actor, taskId, cancellationToken);
            return TypedResults.Ok(project.ToWeb());
        }
        catch (ProjectArchivedException exception)
        {
            return TypedResults.BadRequest(new ProblemDetails { Detail = exception.Message });
        }
        catch (TaskOpenException exception)
        {
            return TypedResults.BadRequest(new ProblemDetails { Detail = exception.Message });
        }
        catch (ManagerRequiredException)
        {
            return TypedResults.Unauthorized();
        }
        catch (TaskNotFoundException exception)
        {
            return TypedResults.NotFound(new ProblemDetails { Detail = exception.Message });
        }
    }

    private static async Task<Results<
            Ok<List<HistoryEntryRepresentation<TaskAction, TaskRepresentation>>>,
            UnauthorizedHttpResult,
            NotFound<ProblemDetails>>>
        GetTaskHistoryAsync(
            ClaimsPrincipal principal,
            [FromServices] TaskGetHistoryUseCase useCase,
            [FromRoute] Guid taskId,
            CancellationToken cancellationToken = default)
    {
        try
        {
            var actor = principal.ToActor();
            var history = await useCase.GetTaskHistoryAsync(actor, taskId, cancellationToken);
            return TypedResults.Ok(history.Select(entry => entry.ToWeb()).ToList());
        }
        catch (ProjectMemberException)
        {
            return TypedResults.Unauthorized();
        }
        catch (ProjectNotFoundException exception)
        {
            return TypedResults.NotFound(new ProblemDetails { Detail = exception.Message });
        }
        catch (TaskNotFoundException exception)
        {
            return TypedResults.NotFound(new ProblemDetails { Detail = exception.Message });
        }
    }

    private static async
        Task<Results<
            Ok<PageRepresentation<TaskRepresentation>>,
            UnauthorizedHttpResult,
            NotFound<ProblemDetails>>>
        ListTasksForProjectAsync(
            ClaimsPrincipal principal,
            [FromServices] TaskListForProjectUseCase useCase,
            [FromRoute] Guid projectId,
            [FromQuery, Range(0, int.MaxValue)] int pageNumber = 0,
            [FromQuery, Range(1, 100)] int pageSize = 10,
            CancellationToken cancellationToken = default)
    {
        try
        {
            var actor = principal.ToActor();
            var pageRequest = new PageRequest(pageNumber, pageSize);
            var page = await useCase.ListTasksForProjectAsync(actor, projectId, pageRequest, cancellationToken);
            return TypedResults.Ok(page.ToWeb());
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

    private static PageRepresentation<TaskRepresentation> ToWeb(this Page<TaskDto> page) =>
        new()
        {
            Size = page.Size,
            TotalElements = page.TotalElements,
            TotalPages = page.TotalPages,
            Number = page.Number,
            Content = [.. page.Content.Select(task => task.ToWeb())]
        };

    private static HistoryEntryRepresentation<TaskAction, TaskRepresentation> ToWeb(
        this HistoryEntry<TaskAction, TaskDto> historyEntry) =>
        new()
        {
            Action = historyEntry.Action,
            Entity = historyEntry.Entity.ToWeb(),
            Timestamp = historyEntry.Timestamp,
            User = new UserRepresentation
            {
                Id = historyEntry.User.Id,
                FirstName = historyEntry.User.FirstName,
                LastName = historyEntry.User.LastName
            }
        };

    private static TaskRepresentation ToWeb(this TaskDto task) =>
        new()
        {
            Id = task.Id,
            DisplayName = task.DisplayName,
            Description = task.Description,
            Open = task.Open,
            Assignees = task.Assignees
        };
}
