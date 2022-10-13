using System.Net;
using FluentAssertions;
using SpecFlow.Demo.Api.Specs.Extensions;

namespace SpecFlow.Demo.Api.Specs.StepDefinitions;

[Binding]
[Scope(Feature = "Backpack update")]
public class BackpackUpdateStepDefinitions : IClassFixture<WebTestServer>
{
    private readonly WebTestServer _webTestServer;
    private HttpClient _backpackOwner;
    private Guid _backpackId;
    private BackpackModel[] _ownerBackpacks;
    private string _backpackName;
    private HttpClient _anotherAuthenticatedUser;
    private HttpResponseMessage _httpResponseMessage;

    public BackpackUpdateStepDefinitions(WebTestServer webTestServer)
    {
        _webTestServer = webTestServer;
    }

    [Given(@"An authenticated user")]
    public async Task GivenAnAuthenticatedUser()
    {
        _backpackOwner = await _webTestServer.CreateAuthenticatedClient();
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
        _ownerBackpacks.Should().Contain(x => x.Id == _backpackId && x.Name == _backpackName);
    }

    [Given(@"Another authenticated user")]
    public async Task GivenAnotherAuthenticatedUser()
    {
        _anotherAuthenticatedUser = await _webTestServer.CreateAuthenticatedClient();
    }

    [When(@"user edits backpack created by other user")]
    public async Task WhenUserEditsBackpackCreatedByOtherUser()
    {
        _httpResponseMessage = await _anotherAuthenticatedUser.PutAsync($"/backpack/{_backpackId}", new { name = "a name" }.ToStringContent());
    }

    [Then(@"gets Forbidden in response")]
    public void ThenGetsForbiddenInResponse()
    {
        _httpResponseMessage.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}