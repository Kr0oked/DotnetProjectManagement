namespace DotnetProjectManagement.ProjectManagement.App.Keycloak;

using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

[SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Global")]
public class KeycloakClientOptions
{
    public const string Key = "KeycloakClient";

    [Required]
    public string Url { get; set; } = "";

    [Required]
    public string Realm { get; set; } = "";

    [Required]
    public string Username { get; set; } = "";

    [Required]
    public string Password { get; set; } = "";
}
