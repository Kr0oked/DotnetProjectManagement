namespace DotnetProjectManagement.ProjectManagement.IntegrationTests;

using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;

[SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Global")]
public class ClaimsProvider
{
    public List<Claim> Claims { get; set; } = [];
}
