namespace DotnetProjectManagement.WebFrontend.Security;

using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication.Internal;

public class ArrayClaimsPrincipalFactory<TAccount>(IAccessTokenProviderAccessor accessor)
    : AccountClaimsPrincipalFactory<TAccount>(accessor)
    where TAccount : RemoteUserAccount
{
    public override async ValueTask<ClaimsPrincipal> CreateUserAsync(
        TAccount account,
        RemoteAuthenticationUserOptions options)
    {
        var user = await base.CreateUserAsync(account, options);

        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        if (user.Identity is not ClaimsIdentity claimsIdentity || account is null)
        {
            return user;
        }

        foreach (var (claimName, claimValue) in account.AdditionalProperties)
        {
            if (claimValue is not JsonElement { ValueKind: JsonValueKind.Array } jsonClaims)
            {
                continue;
            }

            claimsIdentity.RemoveClaim(claimsIdentity.FindFirst(claimName));

            var claims = jsonClaims.EnumerateArray()
                .Select(jsonClaim => new Claim(claimName, jsonClaim.ToString()));

            claimsIdentity.AddClaims(claims);
        }

        return user;
    }
}
