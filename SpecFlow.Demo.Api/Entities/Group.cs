namespace SpecFlow.Demo.Api.Entities;

public class Group : BaseEntity
{
    public string Name { get; set; }
    public Guid AdminId { get; set; }
    public User Admin { get; set; }
    public virtual ICollection<GroupMember> Members { get; set; }
}