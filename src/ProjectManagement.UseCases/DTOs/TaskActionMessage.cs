namespace DotnetProjectManagement.ProjectManagement.UseCases.DTOs;

using Domain.Actions;

public record TaskActionMessage
{
    public required Guid ActorUserId { get; init; }

    public required TaskAction Action { get; init; }

    public required TaskDto Task { get; init; }

    public required ProjectDto Project { get; init; }
}
