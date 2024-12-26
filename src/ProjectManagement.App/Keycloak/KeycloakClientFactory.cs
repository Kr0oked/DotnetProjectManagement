namespace DotnetProjectManagement.ProjectManagement.App.Keycloak;

using FS.Keycloak.RestApiClient.Api;
using FS.Keycloak.RestApiClient.Authentication.ClientFactory;
using FS.Keycloak.RestApiClient.Authentication.Flow;
using FS.Keycloak.RestApiClient.ClientFactory;
using Microsoft.Extensions.Options;

public class KeycloakClientFactory(IOptions<KeycloakClientOptions> options)
{
    public UsersApi GetUsersApi()
    {
        var authenticationFlow = new PasswordGrantFlow
        {
            KeycloakUrl = options.Value.Url,
            Realm = options.Value.Realm,
            UserName = options.Value.Username,
            Password = options.Value.Password
        };

        var httpClient = AuthenticationHttpClientFactory.Create(authenticationFlow);

        return ApiClientFactory.Create<UsersApi>(httpClient);
    }
}
