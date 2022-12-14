using System.Text;
using Newtonsoft.Json;

namespace SpecFlow.Demo.Api.Specs.Extensions;

public static class ObjectExtensions
{
    public static StringContent ToStringContent(this object data)
    {
        string content = JsonConvert.SerializeObject(data);
        return new StringContent(content, Encoding.UTF8, "application/json");
    }
    public static async Task<T> ReadAsAsync<T>(this HttpResponseMessage response)
    {
        response.EnsureSuccessStatusCode();
        var stringResponse = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<T>(stringResponse);
    }
}