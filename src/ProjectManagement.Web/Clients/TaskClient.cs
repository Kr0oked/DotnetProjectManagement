namespace DotnetProjectManagement.ProjectManagement.Web.Clients;

using System.Net.Http.Json;
using Models;

public class TaskClient(HttpClient httpClient)
{
    public async Task<TaskRepresentation> CreateTaskAsync(
        TaskCreateRequest request,
        CancellationToken cancellationToken = default)
    {
        var response = await httpClient.PostAsJsonAsync("tasks", request, cancellationToken);
        response.EnsureSuccessStatusCode();
        var task = await response.Content.ReadFromJsonAsync<TaskRepresentation>(cancellationToken);
        return task!;
    }

    public async Task<TaskRepresentation> GetTaskDetailsAsync(
        Guid taskId,
        CancellationToken cancellationToken = default)
    {
        var task = await httpClient.GetFromJsonAsync<TaskRepresentation>($"tasks/{taskId}", cancellationToken);
        return task!;
    }

    public async Task<TaskRepresentation> UpdateTaskAsync(
        Guid taskId,
        TaskUpdateRequest request,
        CancellationToken cancellationToken = default)
    {
        var response = await httpClient.PutAsJsonAsync($"tasks/{taskId}", request, cancellationToken);
        response.EnsureSuccessStatusCode();
        var task = await response.Content.ReadFromJsonAsync<TaskRepresentation>(cancellationToken);
        return task!;
    }

    public async Task<TaskRepresentation> CloseTaskAsync(Guid taskId, CancellationToken cancellationToken = default)
    {
        var response = await httpClient.PostAsync($"tasks/{taskId}/close", null, cancellationToken);
        response.EnsureSuccessStatusCode();
        var task = await response.Content.ReadFromJsonAsync<TaskRepresentation>(cancellationToken);
        return task!;
    }

    public async Task<TaskRepresentation> ReopenTaskAsync(Guid taskId, CancellationToken cancellationToken = default)
    {
        var response = await httpClient.PostAsync($"tasks/{taskId}/reopen", null, cancellationToken);
        response.EnsureSuccessStatusCode();
        var task = await response.Content.ReadFromJsonAsync<TaskRepresentation>(cancellationToken);
        return task!;
    }

    public async Task<PageRepresentation<TaskRepresentation>> ListTasksForProjectAsync(
        Guid projectId,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var page = await httpClient.GetFromJsonAsync<PageRepresentation<TaskRepresentation>>(
            $"tasks/project/{projectId}?pageNumber={pageNumber}&pageSize={pageSize}",
            cancellationToken);
        return page!;
    }
}
