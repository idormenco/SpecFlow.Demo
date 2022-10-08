using Newtonsoft.Json;
using System.Text;

namespace SpecFlow.Demo.Api.Tests.Extensions;

public static class ObjectExtensions
{
    public static StringContent ToStringContent(this object data)
    {
        string content = JsonConvert.SerializeObject(data);
        return new StringContent(content, Encoding.UTF8, "application/json");
    }
    public static dynamic ParseResponse(this string response)
    {
        return JsonConvert.DeserializeObject<dynamic>(response);
    }
}