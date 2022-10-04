using System;

namespace SpecFlow.Demo.Api.Models.Auth
{
    public sealed record TokenResponseModel(string Token, DateTime UtcExpirationDate);
}