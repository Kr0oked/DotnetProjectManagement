namespace DotnetProjectManagement.ProjectManagement.App.Extensions;

using Domain.Actions;
using UseCases.DTOs;
using Web.Models;

internal static class WebModelsExtensions
{
    public static PageRepresentation<ProjectRepresentation> ToWeb(this Page<ProjectDto> page) =>
        new()
        {
            Size = page.Size,
            TotalElements = page.TotalElements,
            TotalPages = page.TotalPages,
            Number = page.Number,
            Content = [.. page.Content.Select(project => project.ToWeb())]
        };

    public static HistoryEntryRepresentation<ProjectAction, ProjectRepresentation> ToWeb(
        this HistoryEntry<ProjectAction, ProjectDto> historyEntry) =>
        new()
        {
            Action = historyEntry.Action,
            Entity = historyEntry.Entity.ToWeb(),
            Timestamp = historyEntry.Timestamp,
            User = new UserRepresentation
            {
                Id = historyEntry.User.Id,
                FirstName = historyEntry.User.FirstName,
                LastName = historyEntry.User.LastName
            }
        };

    public static ProjectActionMessageRepresentation ToWeb(this ProjectActionMessage message) =>
        new()
        {
            ActorUserId = message.ActorUserId,
            Action = message.Action,
            Project = message.Project.ToWeb()
        };

    public static TaskActionMessageRepresentation ToWeb(this TaskActionMessage message) =>
        new()
        {
            ActorUserId = message.ActorUserId,
            Action = message.Action,
            Task = message.Task.ToWeb(),
            Project = message.Project.ToWeb()
        };

    public static ProjectRepresentation ToWeb(this ProjectDto project) =>
        new()
        {
            Id = project.Id,
            DisplayName = project.DisplayName,
            Archived = project.Archived,
            Members = project.Members
        };

    public static PageRepresentation<TaskRepresentation> ToWeb(this Page<TaskDto> page) =>
        new()
        {
            Size = page.Size,
            TotalElements = page.TotalElements,
            TotalPages = page.TotalPages,
            Number = page.Number,
            Content = [.. page.Content.Select(task => task.ToWeb())]
        };

    public static HistoryEntryRepresentation<TaskAction, TaskRepresentation> ToWeb(
        this HistoryEntry<TaskAction, TaskDto> historyEntry) =>
        new()
        {
            Action = historyEntry.Action,
            Entity = historyEntry.Entity.ToWeb(),
            Timestamp = historyEntry.Timestamp,
            User = new UserRepresentation
            {
                Id = historyEntry.User.Id,
                FirstName = historyEntry.User.FirstName,
                LastName = historyEntry.User.LastName
            }
        };

    public static TaskRepresentation ToWeb(this TaskDto task) =>
        new()
        {
            Id = task.Id,
            DisplayName = task.DisplayName,
            Description = task.Description,
            Open = task.Open,
            Assignees = task.Assignees
        };

    public static PageRepresentation<UserRepresentation> ToWeb(this Page<UserDto> page) =>
        new()
        {
            Size = page.Size,
            TotalElements = page.TotalElements,
            TotalPages = page.TotalPages,
            Number = page.Number,
            Content = [.. page.Content.Select(user => user.ToWeb())]
        };

    public static UserRepresentation ToWeb(this UserDto user) =>
        new()
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName
        };
}
