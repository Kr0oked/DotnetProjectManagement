using DotnetProjectManagement.ProjectManagement.Web.Clients;
using DotnetProjectManagement.WebFrontend;
using DotnetProjectManagement.WebFrontend.Security;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services
    .AddOidcAuthentication(options => builder.Configuration.Bind("OpenIdConnect", options))
    .AddAccountClaimsPrincipalFactory<ArrayClaimsPrincipalFactory<RemoteUserAccount>>();

const string apiClientName = "api";
builder.Services.AddHttpClient(apiClientName, client =>
        client.BaseAddress = new Uri(builder.Configuration["ApiUrl"]!))
    .AddHttpMessageHandler<CustomAuthorizationMessageHandler>();

builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>()
    .CreateClient(apiClientName));

builder.Services.AddTransient<CustomAuthorizationMessageHandler>();
builder.Services.AddTransient<ProjectClient>();
builder.Services.AddTransient<UserClient>();

builder.Services.AddBlazorBootstrap();

await builder.Build().RunAsync();
