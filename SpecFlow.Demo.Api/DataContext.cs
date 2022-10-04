using Microsoft.EntityFrameworkCore;
using SpecFlow.Demo.Api.Entities;
using SpecFlow.Demo.Api.Entities.Configuration;

namespace SpecFlow.Demo.Api
{
    public class DataContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Backpack> Backpacks { get; set; }

        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.HasPostgresExtension("uuid-ossp");

            modelBuilder.ApplyConfiguration(new UserEntityConfiguration());
            modelBuilder.ApplyConfiguration(new BackpackEntityConfiguration());
        }
    }
}