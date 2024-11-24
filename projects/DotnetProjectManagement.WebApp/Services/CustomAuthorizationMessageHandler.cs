namespace DotnetProjectManagement.WebApp.Services;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

public class CustomAuthorizationMessageHandler : AuthorizationMessageHandler
{
    public CustomAuthorizationMessageHandler(
        IAccessTokenProvider provider, NavigationManager navigation, IConfiguration configuration)
        : base(provider, navigation) =>
        this.ConfigureHandler([configuration["ApiUrl"]!]);
}
