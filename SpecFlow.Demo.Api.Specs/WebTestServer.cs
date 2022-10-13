using System.Reflection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SpecFlow.Demo.Api.Specs.Extensions;
using Xunit.Abstractions;

namespace SpecFlow.Demo.Api.Specs;

public class WebTestServer : IDisposable
{
    private readonly TestServer _testServer;
    private bool _disposed;
    private readonly List<HttpClient> _clients = new();

    // Must be set in each test
    public ITestOutputHelper Output { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="WebTestServer"/> class.
    /// </summary>
    public WebTestServer(ITestOutputHelper output)
    {
        Output = output;
        var path = Assembly.GetAssembly(typeof(WebTestServer))!.Location;

        var hostBuilder = new HostBuilder()
            .UseContentRoot(Path.GetDirectoryName(path))
            .ConfigureAppConfiguration(
                options =>
                {
                    options
                        .AddJsonFile("appsettings.json", optional: false)
                        .AddEnvironmentVariables();
                })
            .ConfigureWebHost(
                webHost =>
                {
                    webHost.UseTestServer();
                    webHost.UseStartup<Startup>();
                    webHost.ConfigureLogging(logging =>
                    {
                        logging.ClearProviders(); // Remove other loggers
                        logging.AddXUnit(Output); // Use the ITestOutputHelper instance
                    });
                });

        // Build and start the IHost.
        var host = hostBuilder.Build();

        using var scope = host.Services.CreateScope();
        var dataContext = scope.ServiceProvider.GetRequiredService<DataContext>();
        dataContext.Database.Migrate();

        host.Start();
        _testServer = host.GetTestServer();
    }

    /// <summary>
    /// Finalizes an instance of the <see cref="WebTestServer"/> class.
    /// </summary>
    ~WebTestServer() => Dispose(false);

    public HttpClient CreateHttpClient()
    {
        var client = _testServer.CreateClient();
        _clients.Add(client);
        return client;
    }

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Releases unmanaged and - optionally - managed resources.
    /// </summary>
    /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources;
    /// <c>false</c> to release only unmanaged resources.</param>
    private void Dispose(bool disposing)
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;

        if (!disposing)
        {
            return;
        }

        _clients.ForEach(c => c.Dispose());
        _testServer.Dispose();
    }


    #region secret_sauce
    public async Task<HttpClient> CreateAuthenticatedClient()
    {
        var client = CreateHttpClient();
        var email = $"{Guid.NewGuid()}@mail.com";
        var password = $"{Guid.NewGuid()}";
        var name = $"{Guid.NewGuid()}";
        var request = new UserRegisterModel { Email = email, Name = name, Password = password };
        var response = await client.PostAsync("/user/register", request.ToStringContent());
        var tokenResponse = await response.ReadAsAsync<TokenResponseModel>();

        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {tokenResponse.Token}");
        return client;
    }
    #endregion
}