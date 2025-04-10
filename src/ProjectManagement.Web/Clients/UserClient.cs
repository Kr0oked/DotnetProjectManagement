namespace DotnetProjectManagement.ProjectManagement.Web.Clients;

using System.Net.Http.Json;
using Models;

public class UserClient(HttpClient httpClient)
{
    public async Task<PageRepresentation<UserRepresentation>> ListUsersAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default) =>
        (await httpClient
            .GetFromJsonAsync<PageRepresentation<UserRepresentation>>(
                $"users?pageNumber={pageNumber}&pageSize={pageSize}",
                cancellationToken))!;

    public async Task<UserRepresentation> GetUserDetailsAsync(
        Guid userId,
        CancellationToken cancellationToken = default) =>
        (await httpClient
            .GetFromJsonAsync<UserRepresentation>($"users/{userId}", cancellationToken))!;
}
