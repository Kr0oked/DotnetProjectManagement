using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using DotnetProjectManagement.WebApp;
using DotnetProjectManagement.WebApp.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddOidcAuthentication(options => builder.Configuration.Bind("OpenIdConnect", options));

const string apiClientName = "api";
builder.Services.AddHttpClient(apiClientName, client =>
        client.BaseAddress = new Uri(builder.Configuration["ApiUrl"]!))
    .AddHttpMessageHandler<CustomAuthorizationMessageHandler>();

builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>()
    .CreateClient(apiClientName));

builder.Services.AddTransient<CustomAuthorizationMessageHandler>();
builder.Services.AddTransient<WeatherForecastClient>();

await builder.Build().RunAsync();
