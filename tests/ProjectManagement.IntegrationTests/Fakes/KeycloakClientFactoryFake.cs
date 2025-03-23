namespace DotnetProjectManagement.ProjectManagement.IntegrationTests.Fakes;

using App.Keycloak;
using FS.Keycloak.RestApiClient.Api;

public class KeycloakClientFactoryFake(IUsersApi usersApi) : IKeycloakClientFactory
{
    public IUsersApi GetUsersApi() => usersApi;
}
