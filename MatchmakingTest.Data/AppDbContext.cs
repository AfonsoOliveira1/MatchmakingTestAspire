// Data/AppDbContext.cs
using Microsoft.EntityFrameworkCore;
using MatchmakingTest.Data.Models;

namespace MatchmakingTest.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Player> Players { get; set; }
        public DbSet<Match> Matches { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Player>(entity =>
            {
                entity.HasKey(p => p.Username);
                entity.Property(p => p.Username).IsRequired().HasMaxLength(50);
                entity.Property(p => p.IsOnQueue).HasDefaultValue(false);
                entity.Property(p => p.OnMatch).HasDefaultValue(false);

                entity.HasMany(p => p.MatchHistory)
                      .WithOne()
                      .HasForeignKey("PlayerUsername")
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Match>(entity =>
            {
                entity.HasKey(m => m.Id);
                entity.Property(m => m.Id).IsRequired();
                entity.Property(m => m.Player1).IsRequired().HasMaxLength(50);
                entity.Property(m => m.Player2).IsRequired().HasMaxLength(50);
                entity.Property(m => m.Start).IsRequired();
            });
        }
    }
}