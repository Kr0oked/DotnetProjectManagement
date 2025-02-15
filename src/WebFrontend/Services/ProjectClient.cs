namespace DotnetProjectManagement.WebFrontend.Services;

using System.Net.Http.Json;
using ProjectManagement.Web.Models;

public class ProjectClient(HttpClient httpClient)
{
    public async Task<PageRepresentation<ProjectRepresentation>> ListProjectsAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default) =>
        (await httpClient
            .GetFromJsonAsync<PageRepresentation<ProjectRepresentation>>(
                $"/api/project-management/projects?pageNumber={pageNumber}&pageSize={pageSize}",
                cancellationToken))!;

    public async Task<ProjectRepresentation> CreateProjectAsync(
        ProjectSaveRequest request,
        CancellationToken cancellationToken = default)
    {
        var response = await httpClient.PostAsJsonAsync("/api/project-management/projects", request, cancellationToken);
        response.EnsureSuccessStatusCode();
        var project = await response.Content.ReadFromJsonAsync<ProjectRepresentation>(cancellationToken);
        return project!;
    }

    public async Task<ProjectRepresentation> GetProjectDetailsAsync(
        Guid projectId,
        CancellationToken cancellationToken = default) =>
        (await httpClient
            .GetFromJsonAsync<ProjectRepresentation>(
                $"/api/project-management/projects/{projectId}",
                cancellationToken))!;

    public async Task<ProjectRepresentation> UpdateProjectAsync(
        Guid projectId,
        ProjectSaveRequest request,
        CancellationToken cancellationToken = default)
    {
        var response = await httpClient
            .PutAsJsonAsync($"/api/project-management/projects/{projectId}", request, cancellationToken);
        response.EnsureSuccessStatusCode();
        var project = await response.Content.ReadFromJsonAsync<ProjectRepresentation>(cancellationToken);
        return project!;
    }

    public async Task<ProjectRepresentation> ArchiveProjectAsync(
        Guid projectId,
        CancellationToken cancellationToken = default)
    {
        var response = await httpClient
            .PostAsync($"/api/project-management/projects/{projectId}/archive", null, cancellationToken);
        response.EnsureSuccessStatusCode();
        var project = await response.Content.ReadFromJsonAsync<ProjectRepresentation>(cancellationToken);
        return project!;
    }

    public async Task<ProjectRepresentation> RestoreProjectAsync(
        Guid projectId,
        CancellationToken cancellationToken = default)
    {
        var response = await httpClient
            .PostAsync($"/api/project-management/projects/{projectId}/restore", null, cancellationToken);
        response.EnsureSuccessStatusCode();
        var project = await response.Content.ReadFromJsonAsync<ProjectRepresentation>(cancellationToken);
        return project!;
    }
}
