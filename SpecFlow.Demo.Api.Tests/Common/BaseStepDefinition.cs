using SpecFlow.Demo.Api.Tests.Extensions;

namespace SpecFlow.Demo.Api.Tests.Common;

/// <summary>
/// This is a base class for other step definitions.
/// Do not decorate it with [Binding]!
/// </summary>
public abstract class BaseStepDefinition : IClassFixture<WebServer>
{
    private readonly WebServer _webServer;
    private HttpClient _alex;
    private HttpClient _cristi;
    private HttpClient _irina;
    private HttpClient _ion;

    private Guid _alexId;
    private Guid _cristiId;
    private Guid _irinaId;

    /// <summary>
    /// Alex is an valid, authenticated user
    /// </summary>
    public HttpClient Alex => _alex;
    /// <summary>
    /// Cristi is an valid, authenticated user
    /// </summary>
    public HttpClient Cristi => _cristi;
    /// <summary>
    /// Irina is an valid, authenticated user
    /// </summary>
    public HttpClient Irina => _irina;

    public Guid AlexId => _alexId;
    public Guid CristiId => _cristiId;
    public Guid IrinaId => _irinaId;

    /// <summary>
    /// ion is a not authenticated user
    /// </summary>
    public HttpClient Ion => _ion;

    public BaseStepDefinition(WebServer webServer)
    {
        _webServer = webServer;
    }

    [Given(@"Alex is an authenticated user")]
    public async Task GivenAlexIsAnAuthenticatedUser()
    {
        (_alex, _alexId, _) = await CreateAuthenticatedUser("Alex");
    }

    [Given(@"Cristi is an authenticated user")]
    public async Task GivenCristiIsAnAuthenticatedUser()
    {
        (_cristi, _cristiId, _) = await CreateAuthenticatedUser("Cristi");
    }

    [Given(@"Irina is an authenticated user")]
    public async Task GivenIrinaIsAnAuthenticatedUser()
    {
        (_irina, _irinaId, _) = await CreateAuthenticatedUser("Irina");
    }

    [Given(@"Ion is a non-authenticated user")]
    public void GivenIonIsANonAuthenticatedUser()
    {
        _ion = _webServer.CreateHttpClient();
    }

    /// <summary>
    /// Registers an app user with random name
    /// </summary>
    /// <param name="name">name prefix</param>
    /// <returns>An http client with Authorization header set</returns>
    private async Task<(HttpClient, Guid id, string name)> CreateAuthenticatedUser(string name)
    {
        name = $"{name}_{Guid.NewGuid()}";
        var client = _webServer.CreateHttpClient();
        var email = $"{name}@mail.com";
        var password = $"{Guid.NewGuid()}";
        var request = new UserRegisterModel { Email = email, Name = name, Password = password };
        var response = await client.PostAsync("/user/register", request.ToStringContent());
        var tokenResponse = await response.ReadAsAsync<TokenResponseModel>();

        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {tokenResponse.Token}");
        return (client, tokenResponse.Id, name);
    }
}