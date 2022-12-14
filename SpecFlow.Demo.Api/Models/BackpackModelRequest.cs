using System.ComponentModel.DataAnnotations;

namespace SpecFlow.Demo.Api.Models;

public sealed record BackpackModelRequest
{
    [Required] public string Name { get; init; }
}