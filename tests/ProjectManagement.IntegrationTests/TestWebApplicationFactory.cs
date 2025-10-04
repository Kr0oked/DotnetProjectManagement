namespace DotnetProjectManagement.ProjectManagement.IntegrationTests;

using System.Diagnostics.CodeAnalysis;
using System.Net.Http.Headers;
using MartinCostello.Logging.XUnit;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Moq;
using Testcontainers.MsSql;
using UseCases.Abstractions;
using Xunit;
using Xunit.Abstractions;

[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
public class TestWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram>, IAsyncLifetime,
    ITestOutputHelperAccessor
    where TProgram : class
{
    private readonly MsSqlContainer msSqlContainer = new MsSqlBuilder()
        .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
        .Build();

    public ClaimsProvider ClaimsProvider { get; } = new();
    public Mock<IMessageBroker> MessageBrokerMock { get; } = new();

    public Task InitializeAsync() => this.msSqlContainer.StartAsync();

    public new Task DisposeAsync() => this.msSqlContainer.DisposeAsync().AsTask();

    public ITestOutputHelper? OutputHelper { get; set; }

    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.ConfigureHostConfiguration(configuration =>
            configuration.AddInMemoryCollection(new Dictionary<string, string>
            {
                { "ConnectionStrings:project-management-db", this.msSqlContainer.GetConnectionString() }
            }!));
        return base.CreateHost(builder);
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            services.AddAuthentication(TestAuthHandler.AuthenticationScheme)
                .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(
                    TestAuthHandler.AuthenticationScheme,
                    _ => { });

            services.AddAuthorization(config => config.DefaultPolicy =
                new AuthorizationPolicyBuilder(config.DefaultPolicy)
                    .AddAuthenticationSchemes(TestAuthHandler.AuthenticationScheme)
                    .Build());

            services.Add(new ServiceDescriptor(typeof(ClaimsProvider), this.ClaimsProvider));

            services.Replace(new ServiceDescriptor(typeof(IMessageBroker), this.MessageBrokerMock.Object));
        });

        builder.ConfigureLogging(logging => logging.AddXUnit(this));
        builder.UseEnvironment("Development");
    }

    protected override void ConfigureClient(HttpClient client) =>
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(TestAuthHandler.AuthenticationScheme);
}
