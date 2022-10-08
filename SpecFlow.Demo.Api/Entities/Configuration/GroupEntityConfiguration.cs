using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SpecFlow.Demo.Api.Entities.Configuration;

internal class GroupEntityConfiguration : IEntityTypeConfiguration<Group>
{
    public void Configure(EntityTypeBuilder<Group> builder)
    {
        builder
            .HasKey(x => x.Id);

        builder
            .Property(e => e.Id)
            .HasDefaultValueSql("uuid_generate_v4()");

        builder
            .Property(e => e.Name)
            .HasMaxLength(250)
            .IsRequired();

        builder
            .HasOne(e => e.Admin)
            .WithMany()
            .HasForeignKey(e => e.AdminId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}