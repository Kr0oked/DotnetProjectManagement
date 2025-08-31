namespace DotnetProjectManagement.ProjectManagement.UseCases.Exceptions;

public class TaskClosedException(Guid taskId)
    : Exception($"Task {taskId} is closed");
