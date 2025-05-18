namespace DotnetProjectManagement.AppHost.IntegrationTests;

using Aspire.Hosting;
using Aspire.Hosting.ApplicationModel;
using Aspire.Hosting.Testing;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

// ReSharper disable once ClassNeverInstantiated.Global
public sealed class AppFixture : IAsyncLifetime
{
    public HttpClient GatewayClient { get; private set; } = null!;
    public HttpClient WebFrontendClient { get; private set; } = null!;

    private DistributedApplication distributedApplication = null!;

    public async Task InitializeAsync()
    {
        var appHost = await DistributedApplicationTestingBuilder
            .CreateAsync<Projects.AppHost>();

        appHost.Services.ConfigureHttpClientDefaults(clientBuilder => clientBuilder.AddStandardResilienceHandler());

        this.distributedApplication = await appHost.BuildAsync();

        var resourceNotificationService = this.distributedApplication.Services
            .GetRequiredService<ResourceNotificationService>();

        await this.distributedApplication.StartAsync();

        this.GatewayClient = this.distributedApplication.CreateHttpClient("gateway");
        this.WebFrontendClient = this.distributedApplication.CreateHttpClient("web-frontend");

        await resourceNotificationService
            .WaitForResourceAsync("web-frontend", KnownResourceStates.Running)
            .WaitAsync(TimeSpan.FromSeconds(30));
    }

    public async Task DisposeAsync() => await this.distributedApplication.DisposeAsync();
}
