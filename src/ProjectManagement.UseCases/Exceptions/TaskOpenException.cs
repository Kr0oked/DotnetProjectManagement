namespace DotnetProjectManagement.ProjectManagement.UseCases.Exceptions;

public class TaskOpenException(Guid taskId)
    : Exception($"Task {taskId} is open");
