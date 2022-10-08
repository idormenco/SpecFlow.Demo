using SpecFlow.Demo.Api.Tests.Extensions;

namespace SpecFlow.Demo.Api.Tests.Drivers;

public static class UserDriver
{
    public static async Task<(string email, string password, string token)> RegisterUserAsync( HttpClient client)
    {
        var email = $"{Guid.NewGuid()}@mail.com";
        var password = $"{Guid.NewGuid()}";
        var name = $"{Guid.NewGuid()}";
        var response = await client.PostAsync("/user/register", new { email, password, name }.ToStringContent());
        response.EnsureSuccessStatusCode();

        var stringContent = await response.Content.ReadAsStringAsync();
        string token = stringContent.ParseResponse().token!.ToString()!;
        return (email, password, token);

    }
}