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
            modelBuilder.Entity<Agent>(builder => builder.HasMany(a => a.Services));
            modelBuilder.Entity<Agent>(builder => builder.Property(a => a.Name));
            modelBuilder.Entity<Agent>(builder => builder.Property(a => a.Address));

            modelBuilder.Entity<Service>(builder => builder.HasKey(a => a.Id));
            modelBuilder.Entity<Service>(builder => builder.Property(a => a.Name));
            modelBuilder.Entity<Service>(builder => builder
                                                        .HasOne(a => a.Agent)
                                                        .WithMany(x=>x.Services)
                                                        .HasForeignKey(x=>x.AgentId));

            modelBuilder.Entity<Service>(builder => builder.Property(a => a.Status));
            modelBuilder.Entity<Service>(builder => builder.Property(a => a.LogOnAs));
            modelBuilder.Entity<Service>(builder => builder.Property(a => a.Description));
            modelBuilder.Entity<Service>(builder => builder.HasMany(s => s.Events));
            
            modelBuilder.Entity<ServiceEvent>(builder => builder.HasKey(s => s.Id));
            modelBuilder.Entity<ServiceEvent>(builder => builder.Property(s => s.Status));
            modelBuilder.Entity<ServiceEvent>(builder => builder.Property(s => s.CreatedAt));
            modelBuilder.Entity<ServiceEvent>(builder => builder.HasOne(s => s.Service)
                .WithMany(x => x.Events)
                .HasForeignKey(y => y.ServiceId));

        }

        public DbSet<Agent> Agents { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<ServiceEvent> ServiceEvents { get; set; }
    }
}