using Gadget.Server.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Gadget.Server.Persistence
{
    public class GadgetContext : DbContext
    {
        public GadgetContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Agent>(builder => builder.HasKey(a => a.Id));
            modelBuilder.Entity<Agent>(builder => builder.HasMany(a => a.Services)
                .WithOne(s => s.Agent).HasForeignKey("AgentId"));
            modelBuilder.Entity<Agent>(builder => builder.Property(a => a.Name));
            modelBuilder.Entity<Agent>(builder => builder.Property(a => a.Address));

            modelBuilder.Entity<Service>(builder => builder.HasKey(a => a.Id));
            modelBuilder.Entity<Service>(builder => builder.Property(a => a.Name));
            modelBuilder.Entity<Service>(builder => builder
                .HasOne(a => a.Agent)
                .WithMany(x => x.Services)
                .HasForeignKey("AgentId"));

            modelBuilder.Entity<Service>(builder => builder.Property(a => a.Status));
            modelBuilder.Entity<Service>(builder => builder.Property(a => a.LogOnAs));
            modelBuilder.Entity<Service>(builder => builder.Property(a => a.Description));
            modelBuilder.Entity<Service>(builder => builder.HasMany(s => s.Events));
            modelBuilder.Entity<Service>(builder => builder.HasOne(s => s.Agent));
            modelBuilder.Entity<Service>(builder => builder.OwnsOne(s => s.Config));

            modelBuilder.Entity<ServiceEvent>(builder => builder.HasKey(s => s.Id));
            modelBuilder.Entity<ServiceEvent>(builder => builder.Property(s => s.Status));
            modelBuilder.Entity<ServiceEvent>(builder => builder.Property(s => s.CreatedAt));
            modelBuilder.Entity<ServiceEvent>(builder => builder.HasOne(s => s.Service)
                .WithMany(x => x.Events)
                .HasForeignKey("ServiceId"));

            modelBuilder.Entity<Group>(builder => builder.HasKey(g => g.Id));
            modelBuilder.Entity<Group>(builder => builder.Property(g => g.Name));
            modelBuilder.Entity<Group>(builder => builder.HasMany(g => g.Resources));
            modelBuilder.Entity<UserAction>(builder => builder.HasKey(a => a.Id));
        }

        public DbSet<Agent> Agents { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<ServiceEvent> ServiceEvents { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<UserAction> UserActions { get; set; }
    }
}