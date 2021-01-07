using Gadget.Server.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Gadget.Server
{
    public class GadgetContext : DbContext
    {
        public GadgetContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Agent>(builder => builder.HasKey(a => a.Id));
            modelBuilder.Entity<Agent>(builder => builder.HasMany(a => a.Services));
            modelBuilder.Entity<Service>(builder => builder.HasKey(a => a.Id));
            modelBuilder.Entity<Service>(builder => builder.Property(a => a.Name));
            modelBuilder.Entity<Service>(builder => builder.Property(a => a.Status));
            modelBuilder.Entity<Service>(builder => builder.Property(a => a.LogOnAs));
            modelBuilder.Entity<Service>(builder => builder.Property(a => a.Description));
            modelBuilder.Entity<Agent>(builder => builder.Property(a => a.Name));
            modelBuilder.Entity<Agent>(builder => builder.Property(a => a.Address));
        }

        public DbSet<Agent> Agents { get; set; }
        public DbSet<Service> Services { get; set; }
    }
}