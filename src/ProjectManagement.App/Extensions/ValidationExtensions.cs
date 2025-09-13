namespace DotnetProjectManagement.ProjectManagement.App.Extensions;

using System.ComponentModel.DataAnnotations;

internal static class ValidationExtensions
{
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
