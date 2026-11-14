using DaHorta.Api.Application.Contracts;

namespace DaHorta.Api.Application.Abstractions;

public interface IListingService
{
    Task<IEnumerable<ListingSummaryDto>> GetListingsAsync(double? latitude, double? longitude, double? maxDistanceKm, CancellationToken cancellationToken = default);
    Task<ListingDetailDto?> GetByIdAsync(Guid id, double? latitude, double? longitude, CancellationToken cancellationToken = default);
    Task<ListingDetailDto> CreateAsync(CreateListingRequest request, CancellationToken cancellationToken = default);
    Task<ReservationDto> ReserveAsync(Guid listingId, CreateReservationRequest request, CancellationToken cancellationToken = default);
}
