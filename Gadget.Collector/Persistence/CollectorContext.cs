using Gadget.Collector.Events;
using Gadget.Collector.Metrics;
using Microsoft.EntityFrameworkCore;

namespace Gadget.Collector.Persistence
{
    public class CollectorContext : DbContext
    {
        public CollectorContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<BasicMetric>(b => b.HasKey(m => m.Id));
            modelBuilder.Entity<ServiceStatusChangedEvent>(b => b.HasKey(m => m.Id));
        }

        public DbSet<BasicMetric> BasicMetrics { get; set; }
        public DbSet<ServiceStatusChangedEvent> StatusChangedEvents { get; set; }
    }
}