namespace DotnetProjectManagement.ProjectManagement.Web.Models;

using System.ComponentModel;

public record UserRepresentation
{
    [Description("ID of the user.")]
    public required Guid Id { get; init; }

    [Description("First name of the user.")]
    public string? FirstName { get; init; }

    [Description("Last name of the user.")]
    public string? LastName { get; init; }
}
