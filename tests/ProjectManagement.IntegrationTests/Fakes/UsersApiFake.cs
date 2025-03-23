namespace DotnetProjectManagement.ProjectManagement.IntegrationTests.Fakes;

using FS.Keycloak.RestApiClient.Api;
using FS.Keycloak.RestApiClient.Client;
using FS.Keycloak.RestApiClient.Model;

public class UsersApiFake : IUsersApi
{
    public List<UserRepresentation> Users { get; set; } = [];

    public string GetBasePath() => throw new NotImplementedException();

    public IReadableConfiguration Configuration { get; set; } = null!;
    public ExceptionFactory ExceptionFactory { get; set; } = null!;

    public int GetUsersCount(string realm, string email = null!, bool? emailVerified = null, bool? enabled = null,
        string firstName = null!, string lastName = null!, string q = null!, string search = null!,
        string username = null!) =>
        throw new NotImplementedException();

    public ApiResponse<int> GetUsersCountWithHttpInfo(string realm, string email = null!, bool? emailVerified = null,
        bool? enabled = null, string firstName = null!, string lastName = null!, string q = null!,
        string search = null!, string username = null!) =>
        throw new NotImplementedException();

    public List<UserRepresentation> GetUsers(string realm, bool? briefRepresentation = null, string email = null!,
        bool? emailVerified = null, bool? enabled = null, bool? exact = null, int? first = null,
        string firstName = null!, string idpAlias = null!, string idpUserId = null!, string lastName = null!,
        int? max = null, string q = null!, string search = null!, string username = null!) =>
        throw new NotImplementedException();

    public ApiResponse<List<UserRepresentation>> GetUsersWithHttpInfo(string realm, bool? briefRepresentation = null,
        string email = null!, bool? emailVerified = null, bool? enabled = null, bool? exact = null, int? first = null,
        string firstName = null!, string idpAlias = null!, string idpUserId = null!, string lastName = null!,
        int? max = null, string q = null!, string search = null!, string username = null!) =>
        throw new NotImplementedException();

    public void PostUsers(string realm, UserRepresentation userRepresentation = null!) =>
        throw new NotImplementedException();

    public ApiResponse<object> PostUsersWithHttpInfo(string realm, UserRepresentation userRepresentation = null!) =>
        throw new NotImplementedException();

    public UPConfig GetUsersProfile(string realm) => throw new NotImplementedException();

    public ApiResponse<UPConfig> GetUsersProfileWithHttpInfo(string realm) => throw new NotImplementedException();

    public UserProfileMetadata GetUsersProfileMetadata(string realm) => throw new NotImplementedException();

    public ApiResponse<UserProfileMetadata> GetUsersProfileMetadataWithHttpInfo(string realm) =>
        throw new NotImplementedException();

    public UPConfig PutUsersProfile(string realm, UPConfig uPConfig = null!) => throw new NotImplementedException();

    public ApiResponse<UPConfig> PutUsersProfileWithHttpInfo(string realm, UPConfig uPConfig = null!) =>
        throw new NotImplementedException();

    public List<string> GetUsersConfiguredUserStorageCredentialTypesByUserId(string realm, string userId) =>
        throw new NotImplementedException();

    public ApiResponse<List<string>> GetUsersConfiguredUserStorageCredentialTypesByUserIdWithHttpInfo(string realm,
        string userId) => throw new NotImplementedException();

    public void DeleteUsersConsentsByUserIdAndClient(string realm, string userId, string varClient) =>
        throw new NotImplementedException();

    public ApiResponse<object> DeleteUsersConsentsByUserIdAndClientWithHttpInfo(string realm, string userId,
        string varClient) => throw new NotImplementedException();

    public List<Dictionary<string, object>> GetUsersConsentsByUserId(string realm, string userId) =>
        throw new NotImplementedException();

    public ApiResponse<List<Dictionary<string, object>>> GetUsersConsentsByUserIdWithHttpInfo(string realm,
        string userId) => throw new NotImplementedException();

    public void DeleteUsersCredentialsByUserIdAndCredentialId(string realm, string userId, string credentialId) =>
        throw new NotImplementedException();

    public ApiResponse<object> DeleteUsersCredentialsByUserIdAndCredentialIdWithHttpInfo(string realm, string userId,
        string credentialId) => throw new NotImplementedException();

    public void PostUsersCredentialsMoveAfterByUserIdAndCredentialIdAndNewPreviousCredentialId(string realm,
        string userId, string credentialId, string newPreviousCredentialId) =>
        throw new NotImplementedException();

    public ApiResponse<object>
        PostUsersCredentialsMoveAfterByUserIdAndCredentialIdAndNewPreviousCredentialIdWithHttpInfo(string realm,
            string userId, string credentialId, string newPreviousCredentialId) =>
        throw new NotImplementedException();

    public void PostUsersCredentialsMoveToFirstByUserIdAndCredentialId(string realm, string userId,
        string credentialId) => throw new NotImplementedException();

    public ApiResponse<object> PostUsersCredentialsMoveToFirstByUserIdAndCredentialIdWithHttpInfo(string realm,
        string userId, string credentialId) =>
        throw new NotImplementedException();

    public void PutUsersCredentialsUserLabelByUserIdAndCredentialId(string realm, string userId, string credentialId,
        string body = null!) =>
        throw new NotImplementedException();

    public ApiResponse<object> PutUsersCredentialsUserLabelByUserIdAndCredentialIdWithHttpInfo(string realm,
        string userId, string credentialId, string body = null!) =>
        throw new NotImplementedException();

    public List<CredentialRepresentation> GetUsersCredentialsByUserId(string realm, string userId) =>
        throw new NotImplementedException();

    public ApiResponse<List<CredentialRepresentation>> GetUsersCredentialsByUserIdWithHttpInfo(string realm,
        string userId) => throw new NotImplementedException();

    public void DeleteUsersByUserId(string realm, string userId) => throw new NotImplementedException();

    public ApiResponse<object> DeleteUsersByUserIdWithHttpInfo(string realm, string userId) =>
        throw new NotImplementedException();

    public void PutUsersDisableCredentialTypesByUserId(string realm, string userId, List<string> requestBody = null!) =>
        throw new NotImplementedException();

    public ApiResponse<object> PutUsersDisableCredentialTypesByUserIdWithHttpInfo(string realm, string userId,
        List<string> requestBody = null!) => throw new NotImplementedException();

    public void PutUsersExecuteActionsEmailByUserId(string realm, string userId, string clientId = null!,
        int? lifespan = null, string redirectUri = null!, List<string> requestBody = null!) =>
        throw new NotImplementedException();

    public ApiResponse<object> PutUsersExecuteActionsEmailByUserIdWithHttpInfo(string realm, string userId,
        string clientId = null!, int? lifespan = null, string redirectUri = null!, List<string> requestBody = null!) =>
        throw new NotImplementedException();

    public List<FederatedIdentityRepresentation> GetUsersFederatedIdentityByUserId(string realm, string userId) =>
        throw new NotImplementedException();

    public ApiResponse<List<FederatedIdentityRepresentation>>
        GetUsersFederatedIdentityByUserIdWithHttpInfo(string realm, string userId) =>
        throw new NotImplementedException();

    public void DeleteUsersFederatedIdentityByUserIdAndProvider(string realm, string userId, string provider) =>
        throw new NotImplementedException();

    public ApiResponse<object> DeleteUsersFederatedIdentityByUserIdAndProviderWithHttpInfo(string realm, string userId,
        string provider) => throw new NotImplementedException();

    public void PostUsersFederatedIdentityByUserIdAndProvider(string realm, string userId, string provider) =>
        throw new NotImplementedException();

    public ApiResponse<object> PostUsersFederatedIdentityByUserIdAndProviderWithHttpInfo(string realm, string userId,
        string provider) => throw new NotImplementedException();

    public UserRepresentation GetUsersByUserId(string realm, string userId, bool? userProfileMetadata = null) =>
        throw new NotImplementedException();

    public ApiResponse<UserRepresentation> GetUsersByUserIdWithHttpInfo(string realm, string userId,
        bool? userProfileMetadata = null) => throw new NotImplementedException();

    public Dictionary<string, long> GetUsersGroupsCountByUserId(string realm, string userId, string search = null!) =>
        throw new NotImplementedException();

    public ApiResponse<Dictionary<string, long>> GetUsersGroupsCountByUserIdWithHttpInfo(string realm, string userId,
        string search = null!) => throw new NotImplementedException();

    public List<GroupRepresentation> GetUsersGroupsByUserId(string realm, string userId,
        bool? briefRepresentation = null, int? first = null, int? max = null, string search = null!) =>
        throw new NotImplementedException();

    public ApiResponse<List<GroupRepresentation>> GetUsersGroupsByUserIdWithHttpInfo(string realm, string userId,
        bool? briefRepresentation = null, int? first = null, int? max = null, string search = null!) =>
        throw new NotImplementedException();

    public void DeleteUsersGroupsByUserIdAndGroupId(string realm, string userId, string groupId) =>
        throw new NotImplementedException();

    public ApiResponse<object> DeleteUsersGroupsByUserIdAndGroupIdWithHttpInfo(string realm, string userId,
        string groupId) => throw new NotImplementedException();

    public void PutUsersGroupsByUserIdAndGroupId(string realm, string userId, string groupId) =>
        throw new NotImplementedException();

    public ApiResponse<object>
        PutUsersGroupsByUserIdAndGroupIdWithHttpInfo(string realm, string userId, string groupId) =>
        throw new NotImplementedException();

    public Dictionary<string, object> PostUsersImpersonationByUserId(string realm, string userId) =>
        throw new NotImplementedException();

    public ApiResponse<Dictionary<string, object>> PostUsersImpersonationByUserIdWithHttpInfo(string realm,
        string userId) => throw new NotImplementedException();

    public void PostUsersLogoutByUserId(string realm, string userId) => throw new NotImplementedException();

    public ApiResponse<object> PostUsersLogoutByUserIdWithHttpInfo(string realm, string userId) =>
        throw new NotImplementedException();

    public List<UserSessionRepresentation> GetUsersOfflineSessionsByUserIdAndClientUuid(string realm, string userId,
        string clientUuid) => throw new NotImplementedException();

    public ApiResponse<List<UserSessionRepresentation>> GetUsersOfflineSessionsByUserIdAndClientUuidWithHttpInfo(
        string realm, string userId, string clientUuid) => throw new NotImplementedException();

    public void PutUsersByUserId(string realm, string userId, UserRepresentation userRepresentation = null!) =>
        throw new NotImplementedException();

    public ApiResponse<object> PutUsersByUserIdWithHttpInfo(string realm, string userId,
        UserRepresentation userRepresentation = null!) => throw new NotImplementedException();

    public void PutUsersResetPasswordEmailByUserId(string realm, string userId, string clientId = null!,
        string redirectUri = null!) => throw new NotImplementedException();

    public ApiResponse<object> PutUsersResetPasswordEmailByUserIdWithHttpInfo(string realm, string userId,
        string clientId = null!, string redirectUri = null!) =>
        throw new NotImplementedException();

    public void PutUsersResetPasswordByUserId(string realm, string userId,
        CredentialRepresentation credentialRepresentation = null!) =>
        throw new NotImplementedException();

    public ApiResponse<object> PutUsersResetPasswordByUserIdWithHttpInfo(string realm, string userId,
        CredentialRepresentation credentialRepresentation = null!) =>
        throw new NotImplementedException();

    public void PutUsersSendVerifyEmailByUserId(string realm, string userId, string clientId = null!,
        int? lifespan = null, string redirectUri = null!) =>
        throw new NotImplementedException();

    public ApiResponse<object> PutUsersSendVerifyEmailByUserIdWithHttpInfo(string realm, string userId,
        string clientId = null!, int? lifespan = null, string redirectUri = null!) =>
        throw new NotImplementedException();

    public List<UserSessionRepresentation> GetUsersSessionsByUserId(string realm, string userId) =>
        throw new NotImplementedException();

    public ApiResponse<List<UserSessionRepresentation>> GetUsersSessionsByUserIdWithHttpInfo(string realm,
        string userId) => throw new NotImplementedException();

    public Dictionary<string, List<string>> GetUsersUnmanagedAttributesByUserId(string realm, string userId) =>
        throw new NotImplementedException();

    public ApiResponse<Dictionary<string, List<string>>> GetUsersUnmanagedAttributesByUserIdWithHttpInfo(string realm,
        string userId) => throw new NotImplementedException();

    public Task<int> GetUsersCountAsync(string realm, string email = null!, bool? emailVerified = null,
        bool? enabled = null, string firstName = null!, string lastName = null!, string q = null!,
        string search = null!, string username = null!, CancellationToken cancellationToken = new()) =>
        Task.FromResult(this.Users.Count);

    public Task<ApiResponse<int>> GetUsersCountWithHttpInfoAsync(string realm, string email = null!,
        bool? emailVerified = null, bool? enabled = null, string firstName = null!, string lastName = null!,
        string q = null!, string search = null!, string username = null!,
        CancellationToken cancellationToken = new()) =>
        throw new NotImplementedException();

    public Task<List<UserRepresentation>> GetUsersAsync(string realm, bool? briefRepresentation = null,
        string email = null!, bool? emailVerified = null, bool? enabled = null, bool? exact = null, int? first = null,
        string firstName = null!, string idpAlias = null!, string idpUserId = null!, string lastName = null!,
        int? max = null, string q = null!, string search = null!, string username = null!,
        CancellationToken cancellationToken = new()) =>
        Task.FromResult(this.Users
            .Skip(first ?? 0)
            .Take(max ?? this.Users.Count)
            .ToList());

    public Task<ApiResponse<List<UserRepresentation>>> GetUsersWithHttpInfoAsync(string realm,
        bool? briefRepresentation = null, string email = null!, bool? emailVerified = null, bool? enabled = null,
        bool? exact = null, int? first = null, string firstName = null!, string idpAlias = null!,
        string idpUserId = null!, string lastName = null!, int? max = null, string q = null!, string search = null!,
        string username = null!, CancellationToken cancellationToken = new()) =>
        throw new NotImplementedException();

    public Task PostUsersAsync(string realm, UserRepresentation userRepresentation = null!,
        CancellationToken cancellationToken = new()) =>
        throw new NotImplementedException();

    public Task<ApiResponse<object>> PostUsersWithHttpInfoAsync(string realm,
        UserRepresentation userRepresentation = null!, CancellationToken cancellationToken = new()) =>
        throw new NotImplementedException();

    public Task<UPConfig> GetUsersProfileAsync(string realm,
        CancellationToken cancellationToken = new()) => throw new NotImplementedException();

    public Task<ApiResponse<UPConfig>> GetUsersProfileWithHttpInfoAsync(string realm,
        CancellationToken cancellationToken = new()) => throw new NotImplementedException();

    public Task<UserProfileMetadata> GetUsersProfileMetadataAsync(string realm,
        CancellationToken cancellationToken = new()) => throw new NotImplementedException();

    public Task<ApiResponse<UserProfileMetadata>> GetUsersProfileMetadataWithHttpInfoAsync(string realm,
        CancellationToken cancellationToken = new()) =>
        throw new NotImplementedException();

    public Task<UPConfig> PutUsersProfileAsync(string realm, UPConfig uPConfig = null!,
        CancellationToken cancellationToken = new()) =>
        throw new NotImplementedException();

    public Task<ApiResponse<UPConfig>> PutUsersProfileWithHttpInfoAsync(string realm, UPConfig uPConfig = null!,
        CancellationToken cancellationToken = new()) =>
        throw new NotImplementedException();

    public Task<List<string>> GetUsersConfiguredUserStorageCredentialTypesByUserIdAsync(string realm, string userId,
        CancellationToken cancellationToken = new()) =>
        throw new NotImplementedException();

    public Task<ApiResponse<List<string>>> GetUsersConfiguredUserStorageCredentialTypesByUserIdWithHttpInfoAsync(
        string realm, string userId, CancellationToken cancellationToken = new()) =>
        throw new NotImplementedException();

    public Task DeleteUsersConsentsByUserIdAndClientAsync(string realm, string userId, string varClient,
        CancellationToken cancellationToken = new()) =>
        throw new NotImplementedException();

    public Task<ApiResponse<object>> DeleteUsersConsentsByUserIdAndClientWithHttpInfoAsync(string realm, string userId,
        string varClient, CancellationToken cancellationToken = new()) =>
        throw new NotImplementedException();

    public Task<List<Dictionary<string, object>>> GetUsersConsentsByUserIdAsync(string realm, string userId,
        CancellationToken cancellationToken = new()) =>
        throw new NotImplementedException();

    public Task<ApiResponse<List<Dictionary<string, object>>>> GetUsersConsentsByUserIdWithHttpInfoAsync(string realm,
        string userId, CancellationToken cancellationToken = new()) =>
        throw new NotImplementedException();

    public Task DeleteUsersCredentialsByUserIdAndCredentialIdAsync(string realm, string userId, string credentialId,
        CancellationToken cancellationToken = new()) =>
        throw new NotImplementedException();

    public Task<ApiResponse<object>> DeleteUsersCredentialsByUserIdAndCredentialIdWithHttpInfoAsync(string realm,
        string userId, string credentialId, CancellationToken cancellationToken = new()) =>
        throw new NotImplementedException();

    public Task PostUsersCredentialsMoveAfterByUserIdAndCredentialIdAndNewPreviousCredentialIdAsync(string realm,
        string userId, string credentialId, string newPreviousCredentialId,
        CancellationToken cancellationToken = new()) =>
        throw new NotImplementedException();

    public Task<ApiResponse<object>>
        PostUsersCredentialsMoveAfterByUserIdAndCredentialIdAndNewPreviousCredentialIdWithHttpInfoAsync(string realm,
            string userId, string credentialId, string newPreviousCredentialId,
            CancellationToken cancellationToken = new()) =>
        throw new NotImplementedException();

    public Task PostUsersCredentialsMoveToFirstByUserIdAndCredentialIdAsync(string realm, string userId,
        string credentialId, CancellationToken cancellationToken = new()) =>
        throw new NotImplementedException();

    public Task<ApiResponse<object>> PostUsersCredentialsMoveToFirstByUserIdAndCredentialIdWithHttpInfoAsync(
        string realm, string userId, string credentialId, CancellationToken cancellationToken = new()) =>
        throw new NotImplementedException();

    public Task PutUsersCredentialsUserLabelByUserIdAndCredentialIdAsync(string realm, string userId,
        string credentialId, string body = null!, CancellationToken cancellationToken = new()) =>
        throw new NotImplementedException();

    public Task<ApiResponse<object>> PutUsersCredentialsUserLabelByUserIdAndCredentialIdWithHttpInfoAsync(string realm,
        string userId, string credentialId, string body = null!, CancellationToken cancellationToken = new()) =>
        throw new NotImplementedException();

    public Task<List<CredentialRepresentation>> GetUsersCredentialsByUserIdAsync(string realm, string userId,
        CancellationToken cancellationToken = new()) =>
        throw new NotImplementedException();

    public Task<ApiResponse<List<CredentialRepresentation>>> GetUsersCredentialsByUserIdWithHttpInfoAsync(string realm,
        string userId, CancellationToken cancellationToken = new()) =>
        throw new NotImplementedException();

    public Task DeleteUsersByUserIdAsync(string realm, string userId,
        CancellationToken cancellationToken = new()) =>
        throw new NotImplementedException();

    public Task<ApiResponse<object>> DeleteUsersByUserIdWithHttpInfoAsync(string realm, string userId,
        CancellationToken cancellationToken = new()) =>
        throw new NotImplementedException();

    public Task PutUsersDisableCredentialTypesByUserIdAsync(string realm, string userId,
        List<string> requestBody = null!, CancellationToken cancellationToken = new()) =>
        throw new NotImplementedException();

    public Task<ApiResponse<object>> PutUsersDisableCredentialTypesByUserIdWithHttpInfoAsync(string realm,
        string userId, List<string> requestBody = null!, CancellationToken cancellationToken = new()) =>
        throw new NotImplementedException();

    public Task PutUsersExecuteActionsEmailByUserIdAsync(string realm, string userId, string clientId = null!,
        int? lifespan = null, string redirectUri = null!, List<string> requestBody = null!,
        CancellationToken cancellationToken = new()) =>
        throw new NotImplementedException();

    public Task<ApiResponse<object>> PutUsersExecuteActionsEmailByUserIdWithHttpInfoAsync(string realm, string userId,
        string clientId = null!, int? lifespan = null, string redirectUri = null!, List<string> requestBody = null!,
        CancellationToken cancellationToken = new()) =>
        throw new NotImplementedException();

    public Task<List<FederatedIdentityRepresentation>> GetUsersFederatedIdentityByUserIdAsync(string realm,
        string userId, CancellationToken cancellationToken = new()) =>
        throw new NotImplementedException();

    public Task<ApiResponse<List<FederatedIdentityRepresentation>>> GetUsersFederatedIdentityByUserIdWithHttpInfoAsync(
        string realm, string userId, CancellationToken cancellationToken = new()) =>
        throw new NotImplementedException();

    public Task DeleteUsersFederatedIdentityByUserIdAndProviderAsync(string realm, string userId, string provider,
        CancellationToken cancellationToken = new()) =>
        throw new NotImplementedException();

    public Task<ApiResponse<object>> DeleteUsersFederatedIdentityByUserIdAndProviderWithHttpInfoAsync(string realm,
        string userId, string provider,
        CancellationToken cancellationToken = new()) =>
        throw new NotImplementedException();

    public Task PostUsersFederatedIdentityByUserIdAndProviderAsync(string realm, string userId, string provider,
        CancellationToken cancellationToken = new()) =>
        throw new NotImplementedException();

    public Task<ApiResponse<object>> PostUsersFederatedIdentityByUserIdAndProviderWithHttpInfoAsync(string realm,
        string userId, string provider, CancellationToken cancellationToken = new()) =>
        throw new NotImplementedException();

    public Task<UserRepresentation> GetUsersByUserIdAsync(string realm, string userId, bool? userProfileMetadata = null,
        CancellationToken cancellationToken = new()) =>
        Task.FromResult(this.Users.FirstOrDefault(user => user.Id == userId) ?? throw new ApiException(404, ""));

    public Task<ApiResponse<UserRepresentation>> GetUsersByUserIdWithHttpInfoAsync(string realm, string userId,
        bool? userProfileMetadata = null, CancellationToken cancellationToken = new()) =>
        throw new NotImplementedException();

    public Task<Dictionary<string, long>> GetUsersGroupsCountByUserIdAsync(string realm, string userId,
        string search = null!, CancellationToken cancellationToken = new()) =>
        throw new NotImplementedException();

    public Task<ApiResponse<Dictionary<string, long>>> GetUsersGroupsCountByUserIdWithHttpInfoAsync(string realm,
        string userId, string search = null!, CancellationToken cancellationToken = new()) =>
        throw new NotImplementedException();

    public Task<List<GroupRepresentation>> GetUsersGroupsByUserIdAsync(string realm, string userId,
        bool? briefRepresentation = null, int? first = null, int? max = null, string search = null!,
        CancellationToken cancellationToken = new()) =>
        throw new NotImplementedException();

    public Task<ApiResponse<List<GroupRepresentation>>> GetUsersGroupsByUserIdWithHttpInfoAsync(string realm,
        string userId, bool? briefRepresentation = null, int? first = null, int? max = null, string search = null!,
        CancellationToken cancellationToken = new()) =>
        throw new NotImplementedException();

    public Task DeleteUsersGroupsByUserIdAndGroupIdAsync(string realm, string userId, string groupId,
        CancellationToken cancellationToken = new()) =>
        throw new NotImplementedException();

    public Task<ApiResponse<object>> DeleteUsersGroupsByUserIdAndGroupIdWithHttpInfoAsync(string realm, string userId,
        string groupId,
        CancellationToken cancellationToken = new()) =>
        throw new NotImplementedException();

    public Task PutUsersGroupsByUserIdAndGroupIdAsync(string realm, string userId, string groupId,
        CancellationToken cancellationToken = new()) =>
        throw new NotImplementedException();

    public Task<ApiResponse<object>> PutUsersGroupsByUserIdAndGroupIdWithHttpInfoAsync(string realm, string userId,
        string groupId,
        CancellationToken cancellationToken = new()) =>
        throw new NotImplementedException();

    public Task<Dictionary<string, object>> PostUsersImpersonationByUserIdAsync(string realm, string userId,
        CancellationToken cancellationToken = new()) =>
        throw new NotImplementedException();

    public Task<ApiResponse<Dictionary<string, object>>> PostUsersImpersonationByUserIdWithHttpInfoAsync(string realm,
        string userId,
        CancellationToken cancellationToken = new()) =>
        throw new NotImplementedException();

    public Task PostUsersLogoutByUserIdAsync(string realm, string userId,
        CancellationToken cancellationToken = new()) =>
        throw new NotImplementedException();

    public Task<ApiResponse<object>> PostUsersLogoutByUserIdWithHttpInfoAsync(string realm, string userId,
        CancellationToken cancellationToken = new()) =>
        throw new NotImplementedException();

    public Task<List<UserSessionRepresentation>> GetUsersOfflineSessionsByUserIdAndClientUuidAsync(string realm,
        string userId, string clientUuid, CancellationToken cancellationToken = new()) =>
        throw new NotImplementedException();

    public Task<ApiResponse<List<UserSessionRepresentation>>>
        GetUsersOfflineSessionsByUserIdAndClientUuidWithHttpInfoAsync(string realm, string userId, string clientUuid,
            CancellationToken cancellationToken = new()) =>
        throw new NotImplementedException();

    public Task PutUsersByUserIdAsync(string realm, string userId, UserRepresentation userRepresentation = null!,
        CancellationToken cancellationToken = new()) =>
        throw new NotImplementedException();

    public Task<ApiResponse<object>> PutUsersByUserIdWithHttpInfoAsync(string realm, string userId,
        UserRepresentation userRepresentation = null!, CancellationToken cancellationToken = new()) =>
        throw new NotImplementedException();

    public Task PutUsersResetPasswordEmailByUserIdAsync(string realm, string userId, string clientId = null!,
        string redirectUri = null!, CancellationToken cancellationToken = new()) =>
        throw new NotImplementedException();

    public Task<ApiResponse<object>> PutUsersResetPasswordEmailByUserIdWithHttpInfoAsync(string realm, string userId,
        string clientId = null!, string redirectUri = null!, CancellationToken cancellationToken = new()) =>
        throw new NotImplementedException();

    public Task PutUsersResetPasswordByUserIdAsync(string realm, string userId,
        CredentialRepresentation credentialRepresentation = null!, CancellationToken cancellationToken = new()) =>
        throw new NotImplementedException();

    public Task<ApiResponse<object>> PutUsersResetPasswordByUserIdWithHttpInfoAsync(string realm, string userId,
        CredentialRepresentation credentialRepresentation = null!, CancellationToken cancellationToken = new()) =>
        throw new NotImplementedException();

    public Task PutUsersSendVerifyEmailByUserIdAsync(string realm, string userId, string clientId = null!,
        int? lifespan = null, string redirectUri = null!, CancellationToken cancellationToken = new()) =>
        throw new NotImplementedException();

    public Task<ApiResponse<object>> PutUsersSendVerifyEmailByUserIdWithHttpInfoAsync(string realm, string userId,
        string clientId = null!, int? lifespan = null, string redirectUri = null!,
        CancellationToken cancellationToken = new()) =>
        throw new NotImplementedException();

    public Task<List<UserSessionRepresentation>> GetUsersSessionsByUserIdAsync(string realm, string userId,
        CancellationToken cancellationToken = new()) =>
        throw new NotImplementedException();

    public Task<ApiResponse<List<UserSessionRepresentation>>> GetUsersSessionsByUserIdWithHttpInfoAsync(string realm,
        string userId,
        CancellationToken cancellationToken = new()) =>
        throw new NotImplementedException();

    public Task<Dictionary<string, List<string>>> GetUsersUnmanagedAttributesByUserIdAsync(string realm, string userId,
        CancellationToken cancellationToken = new()) =>
        throw new NotImplementedException();

    public Task<ApiResponse<Dictionary<string, List<string>>>> GetUsersUnmanagedAttributesByUserIdWithHttpInfoAsync(
        string realm, string userId, CancellationToken cancellationToken = new()) =>
        throw new NotImplementedException();
}
