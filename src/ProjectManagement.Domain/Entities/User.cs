namespace DotnetProjectManagement.ProjectManagement.Domain.Entities;

public class User
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string? FirstName { get; set; }

    public string? LastName { get; set; }
}
