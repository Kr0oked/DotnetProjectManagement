namespace DotnetProjectManagement.ProjectManagement.App.User;

using System.ComponentModel.DataAnnotations;

public class UserOptions
{
    public const string Key = "User";

    [Required]
    public string Realm { get; set; } = "";
}
