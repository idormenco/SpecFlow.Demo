using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SpecFlow.Demo.Api.Entities.Configuration
{
    internal class UserEntityConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
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
                .Property(e => e.Email)
                .HasMaxLength(250)
                .IsRequired();

            builder
                .Property(e => e.Password)
                .HasMaxLength(250)
                .IsRequired();
        }
    }
}