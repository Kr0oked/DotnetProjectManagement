namespace DotnetProjectManagement.WebFrontend.Services;

using System.Net.Http.Json;
using ProjectManagement.Web.Models;

public class UserClient(HttpClient httpClient)
{
    public async Task<PageRepresentation<UserRepresentation>> ListUsersAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default) =>
        (await httpClient
            .GetFromJsonAsync<PageRepresentation<UserRepresentation>>(
                $"/api/project-management/users?pageNumber={pageNumber}&pageSize={pageSize}",
                cancellationToken))!;

    public async Task<UserRepresentation> GetUserDetailsAsync(
        Guid userId,
        CancellationToken cancellationToken = default) =>
        (await httpClient
            .GetFromJsonAsync<UserRepresentation>($"/api/project-management/users/{userId}", cancellationToken))!;
}
