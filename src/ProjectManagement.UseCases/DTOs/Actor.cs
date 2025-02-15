namespace DotnetProjectManagement.ProjectManagement.UseCases.DTOs;

public class Actor
{
    public required Guid UserId { get; init; }
    public required bool IsAdministrator { get; init; }
}
