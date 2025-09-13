using D3code.Models;
using Microsoft.EntityFrameworkCore;

namespace D3code.Data
{
    public class EnerQuantumDbContext : DbContext
    {
        public EnerQuantumDbContext(DbContextOptions<EnerQuantumDbContext> options) : base(options) { }
        public DbSet<Area> Areas { get; set; }
        public DbSet<EnergyUsage> EnergyUsage { get; set; }
        public DbSet<ClimateEvent> ClimateEvents { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Area>()
                .ToTable("Areas")
                .HasKey(a => a.AreaId);

            modelBuilder.Entity<EnergyUsage>()
                .ToTable("EnergyUsage")
                .HasKey(eu => new { eu.Timestamp, eu.AreaId });

            modelBuilder.Entity<ClimateEvent>()
                .ToTable("ClimateEvents")
                .HasKey(ce => new { ce.StartTimestamp, ce.AreaId });

            modelBuilder.Entity<EnergyUsage>()
                .HasOne(eu => eu.Area)
                .WithMany()
                .HasForeignKey(eu => eu.AreaId);

            modelBuilder.Entity<ClimateEvent>()
                .HasOne(ce => ce.Area)
                .WithMany()
                .HasForeignKey(ce => ce.AreaId);
        }


    }
}
