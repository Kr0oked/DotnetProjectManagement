namespace DotnetProjectManagement.ProjectManagement.UseCases.DTOs;

public class HistoryEntry<TAction, TEntity> where TAction : Enum
{
    public required TAction Action { get; init; }
    public required TEntity Entity { get; init; }
    public required DateTime Timestamp { get; init; }
    public required UserDto User { get; init; }

    public HistoryEntry<TAction, TEntityMapped> Map<TEntityMapped>(Func<TEntity, TEntityMapped> converter) =>
        new()
        {
            Action = this.Action,
            Entity = converter.Invoke(this.Entity),
            Timestamp = this.Timestamp,
            User = this.User
        };
}
