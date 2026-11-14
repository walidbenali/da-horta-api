using NetTopologySuite.Geometries;

namespace DaHorta.Api.Domain.Entities;

public sealed class Farm
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public required string Name { get; set; }
    public required string ProducerName { get; set; }
    public required string WhatsApp { get; set; }
    public required string AddressHint { get; set; }
    public required Point Location { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;
    public ICollection<Listing> Listings { get; set; } = [];
}
