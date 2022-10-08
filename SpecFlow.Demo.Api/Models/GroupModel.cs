namespace SpecFlow.Demo.Api.Models;

public sealed record GroupModel
{
    public Guid Id { get; init; }
    public string Name { get; init; }
}