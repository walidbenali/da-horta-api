using DaHorta.Api.Domain.Entities;

namespace DaHorta.Api.Application.Abstractions;

public interface IListingRepository
{
    Task<List<Listing>> GetActiveListingsAsync(CancellationToken cancellationToken = default);
    Task<Listing?> GetByIdAsync(Guid id, bool includeFarm = true, bool includeReservations = false, CancellationToken cancellationToken = default);
    Task AddAsync(Listing listing, CancellationToken cancellationToken = default);
    Task AddReservationAsync(Reservation reservation, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
