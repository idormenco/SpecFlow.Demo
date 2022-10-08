using System.ComponentModel.DataAnnotations;

namespace SpecFlow.Demo.Api.Models;

public sealed record GroupModelRequest
{
    [Required] public string Name { get; init; }
}