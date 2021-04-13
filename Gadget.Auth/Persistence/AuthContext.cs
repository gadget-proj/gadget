using Gadget.Auth.Domain;
using Microsoft.EntityFrameworkCore;

namespace Gadget.Auth.Persistence
{
    public class AuthContext : DbContext
    {
        public AuthContext(DbContextOptions<AuthContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(builder => builder.HasKey(u => u.Id));
            modelBuilder.Entity<User>(builder => builder.HasMany(u => u.RefreshTokens)
                .WithOne(r => r.User).HasForeignKey("UserId"));

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
            modelBuilder.Entity<RefreshToken>(builder => builder.Property(r => r.IpAddress));
        }
    }
}