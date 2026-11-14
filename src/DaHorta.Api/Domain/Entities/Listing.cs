using DaHorta.Api.Domain.Enums;
using NetTopologySuite.Geometries;

namespace DaHorta.Api.Domain.Entities;

public sealed class Listing
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public required Guid FarmId { get; set; }
    public Farm? Farm { get; set; }
    public required string Title { get; set; }
    public string? Description { get; set; }
    public required string ProductType { get; set; }
    public required decimal Quantity { get; set; }
    public required string Unit { get; set; }
    public required decimal PricePerUnit { get; set; }
    public string Currency { get; set; } = "EUR";
    public required Point Location { get; set; }
    public ListingStatus Status { get; set; } = ListingStatus.Active;
    public required DateTimeOffset ExpiresAt { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;
    public ICollection<Reservation> Reservations { get; set; } = [];

    public decimal ReservedQuantity => Reservations
        .Where(x => x.Status != ReservationStatus.Cancelled)
        .Sum(x => x.Quantity);

    public bool IsExpired(DateTimeOffset now) => Status == ListingStatus.Expired || ExpiresAt <= now;
}
