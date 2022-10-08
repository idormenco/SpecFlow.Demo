using Newtonsoft.Json;
using SpecFlow.Demo.Api.Tests.Drivers;
using SpecFlow.Demo.Api.Tests.Extensions;
using System.Net;

namespace SpecFlow.Demo.Api.Tests.StepDefinitions;

[Binding]
public class BackpackDeleteStepDefinitions : IClassFixture<WebServer>
{
    private readonly WebServer _webServer;
    private HttpClient _backpackOwner;
    private Guid _backpackId;
    private dynamic[] _ownerBackpacks;
    private string _backpackName;
    private HttpClient _anotherAuthenticatedUser;
    private HttpResponseMessage _httpResponseMessage;

    public BackpackDeleteStepDefinitions(WebServer webServer)
    {
        _webServer = webServer;
    }

    [Given(@"An authenticated user")]
    public async Task GivenAnAuthenticatedUser()
    {
        _backpackOwner = _webServer.CreateHttpClient();
        var (_, _, token) = await UserDriver.RegisterUserAsync(_backpackOwner);
        _backpackOwner.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
    }

    [Given(@"user creates a backpack named ""([^""]*)""")]
    public async Task GivenUserCreatesABackpackNamed(string backpackName)
    {
        _backpackName = backpackName;
        var responseMessage = await _backpackOwner.PostAsync("/backpack", new { name = backpackName }.ToStringContent());
        responseMessage.EnsureSuccessStatusCode();
        var stringResponse = await responseMessage.Content.ReadAsStringAsync();
        _backpackId = Guid.Parse(stringResponse.ParseResponse().id!.ToString()!);
    }

    [When(@"owner deletes created backpack")]
    public async Task WhenOwnerDeletesCreatedBackpack()
    {
        var response = await _backpackOwner.DeleteAsync($"/backpack/{_backpackId}");
        response.EnsureSuccessStatusCode();
    }

    [When(@"owner queries for backpacks")]
    public async Task WhenOwnerQueriesForBackpacks()
    {
        var result = await _backpackOwner.GetAsync("/backpacks");
        result.EnsureSuccessStatusCode();
        var content = await result.Content.ReadAsStringAsync();
        _ownerBackpacks = JsonConvert.DeserializeObject<dynamic[]>(content);
    }

    [Then(@"returned backpacks not contain deleted backpack")]
    public void ThenReturnedBackpacksNotContainDeletedBackpack()
    {
        Assert.DoesNotContain(_ownerBackpacks, x => x.id.ToString() == _backpackId.ToString() && x.name == _backpackName);
    }

    [Given(@"Another authenticated user")]
    public async Task GivenAnotherAuthenticatedUser()
    {
        _anotherAuthenticatedUser = _webServer.CreateHttpClient();
        var (_, _, token) = await UserDriver.RegisterUserAsync(_anotherAuthenticatedUser);
        _anotherAuthenticatedUser.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
    }

    [When(@"user deletes backpack created by other user")]
    public async Task WhenUserDeletesBackpackCreatedByOtherUser()
    {
        _httpResponseMessage = await _anotherAuthenticatedUser.DeleteAsync($"/backpack/{_backpackId}");
    }

    [Then(@"gets Forbidden in response")]
    public void ThenGetsForbiddenInResponse()
    {
        Assert.Equal(HttpStatusCode.Forbidden, _httpResponseMessage.StatusCode);
    }
}