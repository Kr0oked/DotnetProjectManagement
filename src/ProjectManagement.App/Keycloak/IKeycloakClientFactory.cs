namespace DotnetProjectManagement.ProjectManagement.App.Keycloak;

using FS.Keycloak.RestApiClient.Api;

public interface IKeycloakClientFactory
{
    IUsersApi GetUsersApi();
}
