using System;

namespace SpecFlow.Demo.Api.Models
{
    public sealed record UserModel(Guid Id, string Email, string Name);
}