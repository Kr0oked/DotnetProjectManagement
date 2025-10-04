namespace DotnetProjectManagement.ProjectManagement.UseCases.DTOs;

using Domain.Actions;

public record ProjectActionMessage
{
    public required Guid ActorUserId { get; init; }

    public required ProjectAction Action { get; init; }

    public required ProjectDto Project { get; init; }
}
