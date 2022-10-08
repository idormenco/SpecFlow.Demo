namespace SpecFlow.Demo.Api.Entities;

public class Backpack: BaseEntity
{
    public string Name { get; set; }
    public Guid OwnerId { get; set; }
    public virtual User Owner { get; set; }
}