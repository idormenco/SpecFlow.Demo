using Newtonsoft.Json;
using SpecFlow.Demo.Api.Tests.Drivers;
using SpecFlow.Demo.Api.Tests.Extensions;

namespace SpecFlow.Demo.Api.Tests.StepDefinitions;

[Binding]
[Scope(Feature = "Backpack create")]
public class BackpackCreateStepDefinitions : IClassFixture<WebServer>
{
    private readonly WebServer _webServer;
    private HttpClient? _user;
    private HttpResponseMessage? _operationResponse;
    private Guid? _backpackId;

    public BackpackCreateStepDefinitions(WebServer webServer)
    {
        _webServer = webServer;
    }

    [Given(@"A user")]
    public async Task GivenAUser()
    {
        _user = _webServer.CreateHttpClient();
        var (_, _, token) = await UserDriver.RegisterUserAsync(_user);
        _user.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
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
        _operationResponse!.EnsureSuccessStatusCode();
        var stringResponse = await _operationResponse.Content.ReadAsStringAsync();
        var response = stringResponse.ParseResponse();
        string id = response.id.ToString()!;

        _backpackId = Guid.Parse(id);
        Assert.NotEqual(Guid.Empty, _backpackId);
    }

    [Then(@"newly created backpack appears in his list")]
    public async Task ThenNewlyCreatedBackpackAppearsInHisList()
    {
        var result = await _user!.GetAsync("/backpacks");
        result.EnsureSuccessStatusCode();
        var content = await result.Content.ReadAsStringAsync();
        var backpacks = JsonConvert.DeserializeObject<dynamic[]>(content);

        Assert.Contains(backpacks, x => x.id.ToString() == _backpackId.ToString());
    }
}