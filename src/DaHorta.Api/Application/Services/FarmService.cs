using DaHorta.Api.Application.Abstractions;
using DaHorta.Api.Application.Contracts;
using DaHorta.Api.Domain.Entities;
using NetTopologySuite.Geometries;

namespace DaHorta.Api.Application.Services;

public sealed class FarmService(IFarmRepository farmRepository) : IFarmService
{
    public async Task<FarmDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var farm = await farmRepository.GetByIdAsync(id, cancellationToken);
        return farm is null ? null : ToDto(farm);
    }

    public async Task<FarmDto> CreateAsync(CreateFarmRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.ProducerName) || string.IsNullOrWhiteSpace(request.Name))
        {
            throw new ArgumentException("Nome do produtor e nome da exploração são obrigatórios.");
        }

        if (string.IsNullOrWhiteSpace(request.AddressHint))
        {
            throw new ArgumentException("Indique uma referência rápida do local.");
        }

        if (request.Latitude is < -90 or > 90)
        {
            throw new ArgumentOutOfRangeException(nameof(request.Latitude), "A latitude deve estar entre -90 e 90.");
        }

        if (request.Longitude is < -180 or > 180)
        {
            throw new ArgumentOutOfRangeException(nameof(request.Longitude), "A longitude deve estar entre -180 e 180.");
        }

        var farm = new Farm
        {
            ProducerName = request.ProducerName.Trim(),
            Name = request.Name.Trim(),
            AddressHint = request.AddressHint.Trim(),
            WhatsApp = request.WhatsApp.Trim(),
            Location = new Point(request.Longitude, request.Latitude) { SRID = 4326 }
        };

        await farmRepository.AddAsync(farm, cancellationToken);

        return ToDto(farm);
    }

    private static FarmDto ToDto(Farm farm) => new(
        farm.Id,
        farm.ProducerName,
        farm.Name,
        farm.AddressHint,
        farm.WhatsApp,
        farm.Location.Y,
        farm.Location.X);
}
