using System;

namespace SpecFlow.Demo.Api.Models
{
    public sealed record BackpackModel
    {
        public Guid Id { get; init; }
        public string Name { get; init; }
    }
}