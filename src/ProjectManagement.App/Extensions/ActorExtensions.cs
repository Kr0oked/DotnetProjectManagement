namespace DotnetProjectManagement.ProjectManagement.App.Extensions;

using System.Security.Claims;
using UseCases.DTOs;

internal static class ActorExtensions
{
    public static Actor ToActor(this ClaimsPrincipal principal) =>
        new()
        {
            UserId = new Guid(principal.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier)?.Value
                              ?? throw new MissingClaimException(nameof(Actor.UserId))),
            FirstName = principal.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.GivenName)?.Value
                        ?? throw new MissingClaimException(nameof(Actor.FirstName)),
            LastName = principal.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Surname)?.Value
                       ?? throw new MissingClaimException(nameof(Actor.LastName)),
            IsAdministrator = principal.IsInRole("app_admin")
        };
}

internal sealed class MissingClaimException(string claim) : Exception($"Claim is missing: {claim}");
