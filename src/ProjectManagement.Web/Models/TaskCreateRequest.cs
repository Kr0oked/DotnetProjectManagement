namespace DotnetProjectManagement.ProjectManagement.Web.Models;

public record TaskCreateRequest : TaskUpdateRequest
{
    public required Guid ProjectId { get; init; }
}
