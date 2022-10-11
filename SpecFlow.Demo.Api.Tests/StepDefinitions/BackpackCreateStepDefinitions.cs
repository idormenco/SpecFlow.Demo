using System.Net;
using SpecFlow.Demo.Api.Tests.Extensions;

namespace SpecFlow.Demo.Api.Tests.StepDefinitions;

[Binding]
[Scope(Feature = "Backpack create")]
public class BackpackCreateStepDefinitions : IClassFixture<WebServer>
{
    private readonly WebServer _webServer;
    private HttpClient _user;
    private HttpResponseMessage _operationResponse;
    private Guid _backpackId;

    public BackpackCreateStepDefinitions(WebServer webServer)
    {
        _webServer = webServer;
    }

    [Given(@"An authenticated user")]
    public async Task GivenAUser()
    {
        _user = await CreateAuthenticatedClient();
    }

    [When(@"creates a backpack")]
    public async Task WhenCreatesABackpack()
    {
        var content = new { name = "my cool backpack" }.ToStringContent();
        _operationResponse = await _user!.PostAsync("/backpack", content);
    }

    [Then(@"response contains backpackId")]
    public async Task ThenResponseContainsBackpackId()
    {
        var response = await _operationResponse.ReadAsAsync<BackpackModel>();
        _backpackId = response.Id;
        Assert.NotEqual(Guid.Empty, _backpackId);
    }

    [Then(@"newly created backpack appears in his list")]
    public async Task ThenNewlyCreatedBackpackAppearsInHisList()
    {
        var result = await _user!.GetAsync("/backpacks");
        var backpacks = await result.ReadAsAsync<BackpackModel[]>();

        Assert.Contains(backpacks, x => x.Id == _backpackId);
    }

    [Given(@"An user")]
    public void GivenAnUser()
    {
        _user = _webServer.CreateHttpClient();
    }

    [Then(@"response has 401 status code in response")]
    public void ThenResponseStatusCodeInResponse()
    {
        Assert.Equal(HttpStatusCode.Unauthorized, _operationResponse.StatusCode);
    }

    #region topSecret
    private async Task<HttpClient> CreateAuthenticatedClient()
    {
        var client = _webServer.CreateHttpClient();
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