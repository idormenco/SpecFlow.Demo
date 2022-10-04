using System.Collections.Generic;

namespace SpecFlow.Demo.Api.Entities
{
    public class User : BaseEntity
    {
        public User()
        {
            Backpacks = new HashSet<Backpack>();
        }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public ICollection<Backpack> Backpacks { get; set; }
    }
}