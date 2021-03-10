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
                .WithOne(s=>s.Agent).HasForeignKey("AgentId"));
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

            modelBuilder.Entity<ServiceEvent>(builder => builder.HasKey(s => s.Id));
            modelBuilder.Entity<ServiceEvent>(builder => builder.Property(s => s.Status));
            modelBuilder.Entity<ServiceEvent>(builder => builder.Property(s => s.CreatedAt));
            modelBuilder.Entity<ServiceEvent>(builder => builder.HasOne(s => s.Service)
                .WithMany(x => x.Events)
                .HasForeignKey( "ServiceId"));


            modelBuilder.Entity<User>(builder => builder.HasKey(u => u.Id));
            modelBuilder.Entity<User>(builder => builder.HasMany(u => u.RefreshTokens)
               .WithOne(r => r.User).HasForeignKey("UserId")); ;
            modelBuilder.Entity<User>(builder => builder.Property(u => u.UserName));
            modelBuilder.Entity<User>(builder => builder.Property(u => u.UserProvider));

            modelBuilder.Entity<RefreshToken>(builder => builder.HasKey(a => a.Id));
            modelBuilder.Entity<RefreshToken>(builder => builder
                                                        .HasOne(r => r.User)
                                                        .WithMany(u => u.RefreshTokens)
                                                        .HasForeignKey("UserId"));

            modelBuilder.Entity<RefreshToken>(builder => builder.Property(r => r.Token));
            modelBuilder.Entity<RefreshToken>(builder => builder.Property(r => r.Unvalidated));
            modelBuilder.Entity<RefreshToken>(builder => builder.Property(r => r.Used));
            modelBuilder.Entity<RefreshToken>(builder => builder.Property(r => r.CreateDate));
            modelBuilder.Entity<RefreshToken>(builder => builder.Property(r => r.ExpireDate));
        }

        public DbSet<Agent> Agents { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<ServiceEvent> ServiceEvents { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<RefreshToken> RefreshToken { get; set; }
    }
}