namespace SpecFlow.Demo.Api.Models.Auth;

public sealed record TokenResponseModel(Guid Id, string Name, string Token, DateTime UtcExpirationDate);