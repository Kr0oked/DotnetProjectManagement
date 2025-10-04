namespace DotnetProjectManagement.ProjectManagement.Web.Models;

using Domain.Actions;

public record TaskActionMessageRepresentation
{
    public required Guid ActorUserId { get; init; }

    public required TaskAction Action { get; init; }

    public required TaskRepresentation Task { get; init; }

    public required ProjectRepresentation Project { get; init; }
}
