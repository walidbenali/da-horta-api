using DaHorta.Api.Domain.Enums;

namespace DaHorta.Api.Application.Contracts;

public sealed record CreateFarmRequest(
    string ProducerName,
    string Name,
    string AddressHint,
    string WhatsApp,
    double Latitude,
    double Longitude);

public sealed record FarmDto(
    Guid Id,
    string ProducerName,
    string Name,
    string AddressHint,
    string WhatsApp,
    double Latitude,
    double Longitude);

public sealed record CreateListingRequest(
    Guid FarmId,
    string Title,
    string ProductType,
    decimal Quantity,
    string Unit,
    decimal PricePerUnit,
    string? Description,
    DateTimeOffset? ExpiresAt,
    string? Currency);

public sealed record ListingSummaryDto(
    Guid Id,
    string Title,
    string ProductType,
    decimal Quantity,
    string Unit,
    decimal PricePerUnit,
    string Currency,
    DateTimeOffset ExpiresAt,
    string ProducerName,
    string FarmName,
    string AddressHint,
    double? DistanceMeters);

public sealed record ListingDetailDto(
    Guid Id,
    string Title,
    string ProductType,
    decimal Quantity,
    string Unit,
    decimal PricePerUnit,
    string Currency,
    string? Description,
    DateTimeOffset ExpiresAt,
    ListingStatus Status,
    FarmDto Farm,
    double? DistanceMeters);

public sealed record CreateReservationRequest(
    string ConsumerName,
    string ConsumerPhone,
    decimal Quantity);

public sealed record ReservationDto(
    Guid Id,
    Guid ListingId,
    string ConsumerName,
    string ConsumerPhone,
    decimal Quantity,
    ReservationStatus Status,
    DateTimeOffset CreatedAt);
