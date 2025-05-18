namespace DotnetProjectManagement.ProjectManagement.App.Services;

using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

[SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Global")]
public class UserOptions
{
    public const string Key = "User";

    [Required]
    public string Realm { get; set; } = "";
}
