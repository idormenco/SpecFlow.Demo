using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SpecFlow.Demo.Api.Entities.Configuration
{
    internal class BackpackEntityConfiguration : IEntityTypeConfiguration<Backpack>
    {
        public void Configure(EntityTypeBuilder<Backpack> builder)
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
                .HasOne(x => x.Owner)
                .WithMany(x => x.Backpacks)
                .HasForeignKey(x => x.OwnerId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}