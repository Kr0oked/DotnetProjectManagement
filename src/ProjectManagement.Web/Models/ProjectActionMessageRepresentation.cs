namespace DotnetProjectManagement.ProjectManagement.Web.Models;

using Domain.Actions;

public record ProjectActionMessageRepresentation
{
    public required Guid ActorUserId { get; init; }

    public required ProjectAction Action { get; init; }

    public required ProjectRepresentation Project { get; init; }
}
