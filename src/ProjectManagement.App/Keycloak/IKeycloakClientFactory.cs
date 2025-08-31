namespace DotnetProjectManagement.ProjectManagement.App.Keycloak;

using FS.Keycloak.RestApiClient.Api;

public interface IKeycloakClientFactory
{
    public IUsersApi GetUsersApi();
}
