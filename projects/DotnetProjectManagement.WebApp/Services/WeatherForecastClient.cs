namespace DotnetProjectManagement.WebApp.Services;

using System.Net.Http.Json;
using Project.Common;

public class WeatherForecastClient(HttpClient httpClient)
{
    public async Task<List<WeatherForecast>> GetWeatherForecastsAsync(CancellationToken cancellationToken = default) =>
        (await httpClient.GetFromJsonAsync<List<WeatherForecast>>("/weatherforecast", cancellationToken))!;
}
