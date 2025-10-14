namespace DotnetProjectManagement.ProjectManagement.App.Services;

using ExternalResource.Client;
using ExternalResource.Client.Models;
using Microsoft.Kiota.Abstractions.Authentication;
using Microsoft.Kiota.Http.HttpClientLibrary;

public class ExternalResourceService(HttpClient httpClient)
{
    public async Task<List<User>> GetUsers(CancellationToken cancellationToken = default)
    {
        var authenticationProvider = new AnonymousAuthenticationProvider();

        var requestAdapter = new HttpClientRequestAdapter(
            authenticationProvider: authenticationProvider,
            httpClient: httpClient);

        var client = new ExternalResourceClient(requestAdapter);

        var result = await client.Users.GetAsync(cancellationToken: cancellationToken);

        return result!;
    }
}
