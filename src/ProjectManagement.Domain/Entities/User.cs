namespace DotnetProjectManagement.ProjectManagement.Domain.Entities;

using System.Diagnostics.CodeAnalysis;

[SuppressMessage("ReSharper", "PropertyCanBeMadeInitOnly.Global")]
public class User
{
    public Guid Id { get; init; } = Guid.NewGuid();

    public string? FirstName { get; set; }

    public string? LastName { get; set; }
}
