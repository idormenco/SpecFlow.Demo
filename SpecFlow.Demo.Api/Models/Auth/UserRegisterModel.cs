using System.ComponentModel.DataAnnotations;

namespace SpecFlow.Demo.Api.Models.Auth
{
    public sealed record UserRegisterModel
    {
        [Required] 
        [EmailAddress]
        public string Email { get; set; }

        [Required] public string Password { get; set; }
        [Required] public string Name { get; set; }
    }
}