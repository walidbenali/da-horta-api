using DaHorta.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;

namespace DaHorta.Api.Infrastructure.Persistence;

public sealed class DaHortaDbContext(DbContextOptions<DaHortaDbContext> options) : DbContext(options)
{
    public DbSet<Farm> Farms => Set<Farm>();
    public DbSet<Listing> Listings => Set<Listing>();
    public DbSet<Reservation> Reservations => Set<Reservation>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresExtension("postgis");

        modelBuilder.Entity<Farm>(entity =>
        {
            entity.ToTable("farms");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Name).HasMaxLength(120).IsRequired();
            entity.Property(x => x.ProducerName).HasMaxLength(120).IsRequired();
            entity.Property(x => x.WhatsApp).HasMaxLength(32).IsRequired();
            entity.Property(x => x.AddressHint).HasMaxLength(200).IsRequired();
            entity.Property(x => x.Location)
                .HasColumnType("geography (point, 4326)")
                .IsRequired();
            entity.Property(x => x.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(x => x.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.HasIndex(x => x.Location).HasMethod("GIST");
        });

        modelBuilder.Entity<Listing>(entity =>
        {
            entity.ToTable("listings");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Title).HasMaxLength(120).IsRequired();
            entity.Property(x => x.Description).HasMaxLength(500);
            entity.Property(x => x.ProductType).HasMaxLength(80).IsRequired();
            entity.Property(x => x.Unit).HasMaxLength(30).IsRequired();
            entity.Property(x => x.Currency).HasMaxLength(8).HasDefaultValue("EUR");
            entity.Property(x => x.Quantity).HasPrecision(12, 3);
            entity.Property(x => x.PricePerUnit).HasPrecision(12, 2);
            entity.Property(x => x.Location)
                .HasColumnType("geography (point, 4326)")
                .IsRequired();
            entity.Property(x => x.Status).HasConversion<string>();
            entity.Property(x => x.ExpiresAt).IsRequired();
            entity.Property(x => x.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(x => x.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.HasOne(x => x.Farm)
                .WithMany(x => x.Listings)
                .HasForeignKey(x => x.FarmId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasIndex(x => x.Status);
            entity.HasIndex(x => x.ExpiresAt);
            entity.HasIndex(x => x.Location).HasMethod("GIST");
        });

        modelBuilder.Entity<Reservation>(entity =>
        {
            entity.ToTable("reservations");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.ConsumerName).HasMaxLength(120).IsRequired();
            entity.Property(x => x.ConsumerPhone).HasMaxLength(32).IsRequired();
            entity.Property(x => x.Quantity).HasPrecision(12, 3);
            entity.Property(x => x.Status).HasConversion<string>();
            entity.Property(x => x.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.HasOne(x => x.Listing)
                .WithMany(x => x.Reservations)
                .HasForeignKey(x => x.ListingId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasIndex(x => x.ListingId);
            entity.HasIndex(x => x.Status);
        });

        base.OnModelCreating(modelBuilder);
    }
}
