using System;

namespace SpecFlow.Demo.Api.Entities
{
    public class Backpack: BaseEntity
    {
        public string Name { get; set; }
        public Guid OwnerId { get; set; }
        public User Owner { get; set; }
    }
}