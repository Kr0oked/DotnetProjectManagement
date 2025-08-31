namespace DotnetProjectManagement.ProjectManagement.UseCases.Exceptions;

public class TaskNotFoundException(Guid taskId)
    : Exception($"Could not find task {taskId}");
