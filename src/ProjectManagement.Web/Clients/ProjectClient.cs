namespace DotnetProjectManagement.ProjectManagement.Web.Clients;

using System.Net.Http.Json;
using Models;

public class ProjectClient(HttpClient httpClient)
{
    public async Task<PageRepresentation<ProjectRepresentation>> ListProjectsAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var page = await httpClient.GetFromJsonAsync<PageRepresentation<ProjectRepresentation>>(
            $"projects?pageNumber={pageNumber}&pageSize={pageSize}",
            cancellationToken);
        return page!;
    }

    public async Task<ProjectRepresentation> CreateProjectAsync(
        ProjectSaveRequest request,
        CancellationToken cancellationToken = default)
    {
        var response = await httpClient.PostAsJsonAsync("projects", request, cancellationToken);
        response.EnsureSuccessStatusCode();
        var project = await response.Content.ReadFromJsonAsync<ProjectRepresentation>(cancellationToken);
        return project!;
    }

    public async Task<ProjectRepresentation> GetProjectDetailsAsync(
        Guid projectId,
        CancellationToken cancellationToken = default)
    {
        var project = await httpClient
            .GetFromJsonAsync<ProjectRepresentation>($"projects/{projectId}", cancellationToken);
        return project!;
    }

    public async Task<ProjectRepresentation> UpdateProjectAsync(
        Guid projectId,
        ProjectSaveRequest request,
        CancellationToken cancellationToken = default)
    {
        var response = await httpClient
            .PutAsJsonAsync($"projects/{projectId}", request, cancellationToken);
        response.EnsureSuccessStatusCode();
        var project = await response.Content.ReadFromJsonAsync<ProjectRepresentation>(cancellationToken);
        return project!;
    }

    public async Task<ProjectRepresentation> ArchiveProjectAsync(
        Guid projectId,
        CancellationToken cancellationToken = default)
    {
        var response = await httpClient
            .PostAsync($"projects/{projectId}/archive", null, cancellationToken);
        response.EnsureSuccessStatusCode();
        var project = await response.Content.ReadFromJsonAsync<ProjectRepresentation>(cancellationToken);
        return project!;
    }

    public async Task<ProjectRepresentation> RestoreProjectAsync(
        Guid projectId,
        CancellationToken cancellationToken = default)
    {
        var response = await httpClient
            .PostAsync($"projects/{projectId}/restore", null, cancellationToken);
        response.EnsureSuccessStatusCode();
        var project = await response.Content.ReadFromJsonAsync<ProjectRepresentation>(cancellationToken);
        return project!;
    }
}
