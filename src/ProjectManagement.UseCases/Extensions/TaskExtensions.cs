namespace DotnetProjectManagement.ProjectManagement.UseCases.Extensions;

using Domain.Entities;
using DTOs;

public static class TaskExtensions
{
    public static TaskDto ToDto(this ProjectTask task) => new()
    {
        Id = task.Id,
        DisplayName = task.DisplayName,
        Description = task.Description,
        Open = task.Open,
        Assignees = [.. task.Assignees]
    };
}
