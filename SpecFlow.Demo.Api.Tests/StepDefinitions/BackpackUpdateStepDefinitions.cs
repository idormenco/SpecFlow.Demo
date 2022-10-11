using System.Net;
using SpecFlow.Demo.Api.Tests.Extensions;

namespace SpecFlow.Demo.Api.Tests.StepDefinitions;

[Binding]
[Scope(Feature = "Backpack update")]
public class BackpackUpdateStepDefinitions : IClassFixture<WebServer>
{
    private readonly WebServer _webServer;
    private HttpClient _backpackOwner;
    private Guid _backpackId;
    private BackpackModel[] _ownerBackpacks;
    private string _backpackName;
    private HttpClient _anotherAuthenticatedUser;
    private HttpResponseMessage _httpResponseMessage;

    public BackpackUpdateStepDefinitions(WebServer webServer)
    {
        _webServer = webServer;
    }

    [Given(@"An authenticated user")]
    public async Task GivenAnAuthenticatedUser()
    {
        _backpackOwner = await CreateAuthenticatedClient();
    }

    [Given(@"user creates a backpack named ""([^""]*)""")]
    public async Task GivenUserCreatesABackpackNamed(string backpackName)
    {
        _backpackName = backpackName;
        var responseMessage = await _backpackOwner.PostAsync("/backpack", new { name = backpackName }.ToStringContent());
        var backpack = await responseMessage.ReadAsAsync<BackpackModel>();
        _backpackId = backpack.Id;
    }

    [When(@"owner edits backpack name to ""([^""]*)""")]
    public async Task WhenOwnerEditsBackpackNameTo(string backpackName)
    {
        _backpackName = backpackName;
        var response = await _backpackOwner.PutAsync($"/backpack/{_backpackId}", new { name = backpackName }.ToStringContent());
        response.EnsureSuccessStatusCode();
    }

    [When(@"owner queries for backpacks")]
    public async Task WhenOwnerQueriesForBackpacks()
    {
        var result = await _backpackOwner.GetAsync("/backpacks");
        _ownerBackpacks = await result.ReadAsAsync<BackpackModel[]>();
    }

    [Then(@"returned backpacks contain updated backpack")]
    public void ThenReturnedBackpacksContainUpdatedBackpack()
    {
        Assert.Contains(_ownerBackpacks, (x) => x.Id == _backpackId && x.Name == _backpackName);
    }

    [Given(@"Another authenticated user")]
    public async Task GivenAnotherAuthenticatedUser()
    {
        _anotherAuthenticatedUser = await CreateAuthenticatedClient();
    }

    [When(@"user edits backpack created by other user")]
    public async Task WhenUserEditsBackpackCreatedByOtherUser()
    {
        _httpResponseMessage = await _anotherAuthenticatedUser.PutAsync($"/backpack/{_backpackId}", new { name = "a name" }.ToStringContent());
    }

    [Then(@"gets Forbidden in response")]
    public void ThenGetsForbiddenInResponse()
    {
        Assert.Equal(HttpStatusCode.Forbidden, _httpResponseMessage.StatusCode);
    }

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
}