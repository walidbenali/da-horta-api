using DaHorta.Api.Application.Abstractions;
using DaHorta.Api.Application.Contracts;
using DaHorta.Api.Domain.Entities;
using DaHorta.Api.Domain.Enums;
using NetTopologySuite.Geometries;

namespace DaHorta.Api.Application.Services;

public sealed class ListingService(IListingRepository listingRepository, IFarmRepository farmRepository) : IListingService
{
    public async Task<IEnumerable<ListingSummaryDto>> GetListingsAsync(
        double? latitude,
        double? longitude,
        double? maxDistanceKm,
        CancellationToken cancellationToken = default)
    {
        var listings = await listingRepository.GetActiveListingsAsync(cancellationToken);

        if (latitude.HasValue && longitude.HasValue)
        {
            var point = new Point(longitude.Value, latitude.Value) { SRID = 4326 };

            return listings
                .Select(listing =>
                {
                    double? distanceMeters = listing.Location is null
                        ? null
                        : Math.Round(listing.Location.Distance(point) * 111_320d, 0);

                    return new ListingSummaryDto(
                        listing.Id,
                        listing.Title,
                        listing.ProductType,
                        listing.Quantity,
                        listing.Unit,
                        listing.PricePerUnit,
                        listing.Currency,
                        listing.ExpiresAt,
                        listing.Farm?.ProducerName ?? string.Empty,
                        listing.Farm?.Name ?? string.Empty,
                        listing.Farm?.AddressHint ?? string.Empty,
                        distanceMeters);
                })
                .Where(item => !maxDistanceKm.HasValue || !item.DistanceMeters.HasValue || item.DistanceMeters <= maxDistanceKm.Value * 1000d)
                .OrderBy(item => item.DistanceMeters ?? double.MaxValue)
                .ToList();
        }

        return listings
            .Select(listing => new ListingSummaryDto(
                listing.Id,
                listing.Title,
                listing.ProductType,
                listing.Quantity,
                listing.Unit,
                listing.PricePerUnit,
                listing.Currency,
                listing.ExpiresAt,
                listing.Farm?.ProducerName ?? string.Empty,
                listing.Farm?.Name ?? string.Empty,
                listing.Farm?.AddressHint ?? string.Empty,
                null))
            .ToList();
    }

    public async Task<ListingDetailDto?> GetByIdAsync(Guid id, double? latitude, double? longitude, CancellationToken cancellationToken = default)
    {
        var listing = await listingRepository.GetByIdAsync(id, includeFarm: true, cancellationToken: cancellationToken);

        if (listing is null)
        {
            return null;
        }

        var point = latitude.HasValue && longitude.HasValue
            ? new Point(longitude.Value, latitude.Value) { SRID = 4326 }
            : null;

        double? distanceMeters = point is null || listing.Location is null
            ? null
            : Math.Round(listing.Location.Distance(point) * 111_320d, 0);

        return new ListingDetailDto(
            listing.Id,
            listing.Title,
            listing.ProductType,
            listing.Quantity,
            listing.Unit,
            listing.PricePerUnit,
            listing.Currency,
            listing.Description,
            listing.ExpiresAt,
            listing.Status,
            new FarmDto(
                listing.Farm!.Id,
                listing.Farm.ProducerName,
                listing.Farm.Name,
                listing.Farm.AddressHint,
                listing.Farm.WhatsApp,
                listing.Farm.Location.Y,
                listing.Farm.Location.X),
            distanceMeters);
    }

    public async Task<ListingDetailDto> CreateAsync(CreateListingRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.Title))
        {
            throw new ArgumentException("O título da oferta é obrigatório.");
        }

        if (request.Quantity <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(request.Quantity), "A quantidade deve ser maior que zero.");
        }

        if (request.PricePerUnit <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(request.PricePerUnit), "O preço por unidade deve ser maior que zero.");
        }

        var farm = await farmRepository.GetByIdAsync(request.FarmId, cancellationToken);
        if (farm is null)
        {
            throw new KeyNotFoundException("Exploração não encontrada.");
        }

        var expiresAt = request.ExpiresAt ?? DateTimeOffset.UtcNow.AddHours(24);
        if (expiresAt <= DateTimeOffset.UtcNow)
        {
            throw new ArgumentException("A data de expiração deve estar no futuro.");
        }

        var listing = new Listing
        {
            FarmId = farm.Id,
            Title = request.Title.Trim(),
            ProductType = request.ProductType.Trim(),
            Description = string.IsNullOrWhiteSpace(request.Description) ? null : request.Description.Trim(),
            Quantity = request.Quantity,
            Unit = string.IsNullOrWhiteSpace(request.Unit) ? "kg" : request.Unit.Trim(),
            PricePerUnit = request.PricePerUnit,
            Currency = string.IsNullOrWhiteSpace(request.Currency) ? "EUR" : request.Currency.Trim().ToUpperInvariant(),
            ExpiresAt = expiresAt,
            Location = new Point(farm.Location.X, farm.Location.Y) { SRID = 4326 },
            Status = ListingStatus.Active
        };

        await listingRepository.AddAsync(listing, cancellationToken);

        return await GetByIdAsync(listing.Id, null, null, cancellationToken)
            ?? throw new InvalidOperationException("A oferta acabou de ser criada e não pôde ser carregada.");
    }

    public async Task<ReservationDto> ReserveAsync(Guid listingId, CreateReservationRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.ConsumerName))
        {
            throw new ArgumentException("O nome do cliente é obrigatório.");
        }

        if (string.IsNullOrWhiteSpace(request.ConsumerPhone))
        {
            throw new ArgumentException("O contacto do cliente é obrigatório.");
        }

        if (request.Quantity <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(request.Quantity), "A quantidade reservada deve ser maior que zero.");
        }

        var listing = await listingRepository.GetByIdAsync(listingId, includeFarm: false, includeReservations: true, cancellationToken);
        if (listing is null)
        {
            throw new KeyNotFoundException("Oferta não encontrada.");
        }

        if (listing.IsExpired(DateTimeOffset.UtcNow))
        {
            listing.Status = ListingStatus.Expired;
            await listingRepository.SaveChangesAsync(cancellationToken);
            throw new InvalidOperationException("Esta oferta já expirou.");
        }

        var reservedQuantity = listing.ReservedQuantity;
        if (request.Quantity > listing.Quantity - reservedQuantity)
        {
            throw new InvalidOperationException("Quantidade indisponível para reserva.");
        }

        var reservation = new Reservation
        {
            ListingId = listing.Id,
            ConsumerName = request.ConsumerName.Trim(),
            ConsumerPhone = request.ConsumerPhone.Trim(),
            Quantity = request.Quantity,
            Status = ReservationStatus.Pending
        };

        await listingRepository.AddReservationAsync(reservation, cancellationToken);

        if (Math.Abs((decimal)(listing.Quantity - reservedQuantity - request.Quantity)) < 0.0001m)
        {
            listing.Status = ListingStatus.Reserved;
            await listingRepository.SaveChangesAsync(cancellationToken);
        }

        return new ReservationDto(
            reservation.Id,
            reservation.ListingId,
            reservation.ConsumerName,
            reservation.ConsumerPhone,
            reservation.Quantity,
            reservation.Status,
            reservation.CreatedAt);
    }
}
