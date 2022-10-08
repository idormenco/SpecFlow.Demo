using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace SpecFlow.Demo.Api.Entities.Configuration;

public class GroupUserEntityConfiguration : IEntityTypeConfiguration<GroupMember>
{
    public void Configure(EntityTypeBuilder<GroupMember> builder)
    {
        builder
            .HasKey(x => x.Id);

        builder
            .Property(e => e.Id)
            .HasDefaultValueSql("uuid_generate_v4()");

        builder.HasKey(e => new { e.GroupId, e.UserId });

        builder
            .HasOne(e => e.User)
            .WithMany()
            .HasForeignKey(e => e.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder
            .HasOne(e => e.Group)
            .WithMany(e => e.Members)
            .HasForeignKey(e => e.GroupId)
            .OnDelete(DeleteBehavior.Cascade);

    }
}