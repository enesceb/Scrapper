using Microsoft.EntityFrameworkCore;
using MeshcapadeDataScraper.Models;

namespace MeshcapadeDataScraper.Data;

public class MeshcapadeDbContext : DbContext
{
    public DbSet<MeasurementData> MeasurementData { get; set; }

    public MeshcapadeDbContext(DbContextOptions<MeshcapadeDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<MeasurementData>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Height).HasPrecision(18, 2);
            entity.Property(e => e.Weight).HasPrecision(18, 2);
            entity.Property(e => e.Chest).HasPrecision(18, 2);
            entity.Property(e => e.Waist).HasPrecision(18, 2);
            entity.Property(e => e.Hip).HasPrecision(18, 2);
            entity.Property(e => e.Inseam).HasPrecision(18, 2);
            entity.Property(e => e.Timestamp).HasDefaultValueSql("datetime('now')");
        });

        base.OnModelCreating(modelBuilder);
    }
} 