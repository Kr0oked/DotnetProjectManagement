namespace DotnetProjectManagement.WebFrontend.Security;

using System.Security.Claims;

internal static class ClaimsPrincipalExtensions
{
    public static Guid? Id(this ClaimsPrincipal claimsPrincipal)
    {
        var sub = claimsPrincipal.FindFirst(claim => claim.Type == "sub");
        if (sub == null)
        {
            return null;
        }

        return Guid.Parse(sub.Value);
    }

    public static bool IsAdmin(this ClaimsPrincipal claimsPrincipal) => claimsPrincipal.IsInRole("app_admin");
}
