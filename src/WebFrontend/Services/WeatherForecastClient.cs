namespace WebFrontend.Services;

using System.Net.Http.Json;
using ProjectManagement.Web.DTOs;

public class WeatherForecastClient(HttpClient httpClient)
{
    public async Task<List<WeatherForecast>> GetWeatherForecastsAsync(CancellationToken cancellationToken = default) =>
        (await httpClient
            .GetFromJsonAsync<List<WeatherForecast>>("/api/project-management/weatherforecast", cancellationToken))!;
}
