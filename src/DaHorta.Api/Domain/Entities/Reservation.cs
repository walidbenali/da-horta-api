using DaHorta.Api.Domain.Enums;

namespace DaHorta.Api.Domain.Entities;

public sealed class Reservation
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public required Guid ListingId { get; set; }
    public Listing? Listing { get; set; }
    public required string ConsumerName { get; set; }
    public required string ConsumerPhone { get; set; }
    public required decimal Quantity { get; set; }
    public ReservationStatus Status { get; set; } = ReservationStatus.Pending;
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}
