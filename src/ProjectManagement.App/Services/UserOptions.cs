namespace DotnetProjectManagement.ProjectManagement.App.Services;

using System.ComponentModel.DataAnnotations;

public class UserOptions
{
    public const string Key = "User";

    [Required]
    public string Realm { get; set; } = "";
}
