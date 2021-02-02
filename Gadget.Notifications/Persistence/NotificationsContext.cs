using System;
using Gadget.Notifications.Domain.Entities;
using Gadget.Notifications.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace Gadget.Notifications.Persistence
{
    public class NotificationsContext : DbContext
    {
        public NotificationsContext(DbContextOptions<NotificationsContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Service>(s => s.HasKey(ss => ss.Id));
            modelBuilder.Entity<Service>(s => s.Property(ss => ss.Name));
            modelBuilder.Entity<Service>(s => s.Property(ss => ss.Agent));
            modelBuilder.Entity<Service>(s => s.HasIndex(ss => ss.Name));
            modelBuilder.Entity<Service>(s => s.OwnsMany(ss => ss.Webhooks, w =>
            {
                w.WithOwner().HasForeignKey("OwnerId");
                w.Property<Guid>("Id");
                w.Property(wh => wh.Uri);
                w.Property(wh => wh.CreatedAt);
                w.HasKey("Id");
            }));

        }

        public DbSet<Service> Services { get; set; }
    }
}