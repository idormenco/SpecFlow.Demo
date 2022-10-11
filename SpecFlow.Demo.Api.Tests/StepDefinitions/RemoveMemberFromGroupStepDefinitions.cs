using SpecFlow.Demo.Api.Tests.Common;
using SpecFlow.Demo.Api.Tests.Extensions;

namespace SpecFlow.Demo.Api.Tests.StepDefinitions;

[Binding]
public class RemoveMemberFromGroupStepDefinitions : BaseStepDefinition
{
    private GroupModel _group;

    public RemoveMemberFromGroupStepDefinitions(WebServer webServer) : base(webServer)
    {
    }

    [Given(@"Alex creates a group")]
    public async Task GivenAlexCreatesAGroup()
    {
        var groupRequest = new GroupModelRequest()
        {
            Name = "Alex's group"
        };

        var response = await Alex.PostAsync("/group", groupRequest.ToStringContent());
        _group = await response.ReadAsAsync<GroupModel>();
    }

    [Given(@"Cristi is a member of Alex's group")]
    public async Task GivenCristiIsAMemberOfAlexsGroup()
    {
        await Alex.PutAsync($"/group/{_group.Id}/invite/{CristiId}", null);
    }

    [When(@"Alex removes Cristi from group")]
    public async Task WhenAlexRemovesCristiFromGroup()
    {
        await Alex.PutAsync($"/group/{_group.Id}/remove/{CristiId}", null);
    }

    [Then(@"the returned group members list does not contain Cristi")]
    public async Task ThenTheReturnedGroupMembersListDoesNotContainCristi()
    {
        var httpResponseMessage = await Alex.GetAsync($"/group/{_group.Id}/members");
        var members = await httpResponseMessage.ReadAsAsync<GroupMemberModel[]>();
        Assert.Single(members);
        Assert.DoesNotContain(members, member => member.Id == CristiId);
    }

}