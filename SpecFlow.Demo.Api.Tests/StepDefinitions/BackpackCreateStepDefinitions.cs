using System.Net;
using FluentAssertions;
using SpecFlow.Demo.Api.Specs.Extensions;

namespace SpecFlow.Demo.Api.Specs.StepDefinitions;

[Binding]
[Scope(Feature = "Backpack create")]
public class BackpackCreateStepDefinitions : IClassFixture<WebTestServer>
{
    private readonly WebTestServer _webTestServer;
    private HttpClient _client;
    private HttpResponseMessage _response;

    public BackpackCreateStepDefinitions(WebTestServer webTestServer)
    {
        _webTestServer = webTestServer;
    }

    [Given(@"an authenticated user")]
    public async Task GivenAUser()
    {
        _client = await _webTestServer.CreateAuthenticatedClient();
    }

    [When(@"user creates a backpack")]
    public async Task WhenUserCreatesABackpack()
    {
        var backpackModelRequest = new BackpackModelRequest { Name = "a backpack" };

        _response = await _client.PostAsync("/backpack", backpackModelRequest.ToStringContent());
    }

    [Then(@"created backpack appears in his backpack list")]
    public async Task ThenCreatedBackpackAppearsInHisBackpackList()
    {
        var response = await _client.GetAsync("/backpacks");
        var backpacks = await response.ReadAsAsync<BackpackModel[]>();
        var backpack = await _response.ReadAsAsync<BackpackModel>();

        backpacks.Should().HaveCount(1);
        backpacks.First().Id.Should().Be(backpack.Id);
        backpacks.First().Name.Should().Be(backpack.Name);
    }

    [Given(@"an non-authenticated user")]
    public void GivenAnNon_AuthenticatedUser()
    {
        _client = _webTestServer.CreateHttpClient();
    }

    [Then(@"401NonAuthenticated response")]
    public void ThenNonAuthenticatedResponse()
    {
        _response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}