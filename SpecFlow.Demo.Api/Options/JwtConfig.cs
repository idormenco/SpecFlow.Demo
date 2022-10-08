namespace SpecFlow.Demo.Api.Options;

public class JwtConfig
{
    public static string SectionKey => "Jwt";

    public string Key { get; set; }
    public string Issuer { get; set; }
    public string Audience { get; set; }
}