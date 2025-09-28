namespace DotnetProjectManagement.ProjectManagement.Web.Models;

using System.ComponentModel;

public record HistoryEntryRepresentation<TA, TE> where TA : Enum
{
    [Description("Action that modified the entity.")]
    public required TA Action { get; init; }

    [Description("The entity at this state.")]
    public required TE Entity { get; init; }

    [Description("Timestamp in UTC when the action was performed.")]
    public required DateTime Timestamp { get; init; }

    [Description("User that performed the action.")]
    public required UserRepresentation User { get; init; }
}
