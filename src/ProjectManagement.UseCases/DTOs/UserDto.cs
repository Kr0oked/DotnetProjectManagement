namespace DotnetProjectManagement.ProjectManagement.UseCases.DTOs;

public record UserDto
{
    public required Guid Id { get; init; }

    public string? FirstName { get; init; }

    public string? LastName { get; init; }
}
