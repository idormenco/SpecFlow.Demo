namespace SpecFlow.Demo.Api.Entities;

public class GroupMember: BaseEntity
{
    public Guid UserId { get; set; }
    public virtual User User { get; set; }
    public Guid GroupId { get; set; }
    public virtual Group Group { get; set; }
}