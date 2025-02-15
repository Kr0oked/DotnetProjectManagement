namespace DotnetProjectManagement.ProjectManagement.App.APIs;

using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using UseCases.DTOs;

internal static class Extensions
{
    public static Actor ToActor(this ClaimsPrincipal principal) =>
        new()
        {
            UserId = new Guid(principal.Identity?.Name ?? throw new PrincipalNameMissingException()),
            IsAdministrator = principal.IsInRole("app_admin")
        };

    public static Dictionary<string, string[]> ToErrorDictionary(this ICollection<ValidationResult> validationResults)
    {
        var errorDictionary = new Dictionary<string, string[]>();

        foreach (var validationResult in validationResults)
        {
            if (validationResult.ErrorMessage == null)
            {
                continue;
            }

            foreach (var memberName in validationResult.MemberNames)
            {
                if (errorDictionary.TryGetValue(memberName, out var value))
                {
                    errorDictionary[memberName] = [.. value, validationResult.ErrorMessage];
                }
                else
                {
                    errorDictionary[memberName] = [validationResult.ErrorMessage];
                }
            }
        }

        return errorDictionary;
    }
}
