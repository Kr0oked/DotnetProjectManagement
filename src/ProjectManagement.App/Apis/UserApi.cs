namespace DotnetProjectManagement.ProjectManagement.App.APIs;

using System.ComponentModel.DataAnnotations;
using Extensions;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using UseCases.DTOs;
using UseCases.Exceptions;
using UseCases.User.GetDetails;
using UseCases.User.List;
using Web.Models;

public static class UserApi
{
    public static RouteGroupBuilder MapUserApi(this IEndpointRouteBuilder app)
    {
        var api = app.MapGroup("users")
            .WithTags("Users");

        api.MapGet("/", ListUsersAsync)
            .WithName("ListUsers")
            .WithSummary("List users")
            .WithDescription("Get a paginated list of users.");
        api.MapGet("/{userId:guid}", GetUserDetailsAsync)
            .WithName("GetUserDetails")
            .WithSummary("Get user")
            .WithDescription("Get details for a user.");

        return api;
    }

    private static async Task<Ok<PageRepresentation<UserRepresentation>>> ListUsersAsync(
        [FromServices] UserListUseCase useCase,
        [FromQuery, Range(0, int.MaxValue)] int pageNumber = 0,
        [FromQuery, Range(1, 100)] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var pageRequest = new PageRequest(pageNumber, pageSize);
        var page = await useCase.ListUsersAsync(pageRequest, cancellationToken);
        return TypedResults.Ok(page.ToWeb());
    }

    private static async Task<Results<Ok<UserRepresentation>, NotFound<ProblemDetails>>> GetUserDetailsAsync(
        [FromServices] UserGetDetailsUseCase useCase,
        [FromRoute] Guid userId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var project = await useCase.GetUserDetailsAsync(userId, cancellationToken);
            return TypedResults.Ok(project.ToWeb());
        }
        catch (UserNotFoundException exception)
        {
            return TypedResults.NotFound(new ProblemDetails { Detail = exception.Message });
        }
    }
}
