namespace DotnetProjectManagement.WebFrontend.Culture;

using System.Globalization;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

public static class WebAssemblyHostExtension
{
    public const string BlazorCultureKey = "BlazorCulture";

    public static async Task SetCulture(this WebAssemblyHost host)
    {
        var localStorageService = host.Services.GetRequiredService<ILocalStorageService>();
        var storedCulture = await localStorageService.GetItemAsStringAsync("BlazorCulture");

        if (storedCulture is not null)
        {
            var culture = CultureInfo.GetCultureInfo(storedCulture);
            CultureInfo.DefaultThreadCurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;
        }
    }
}
