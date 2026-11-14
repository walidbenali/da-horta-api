using DaHorta.Api.Application.Contracts;

namespace DaHorta.Api.Application.Abstractions;

public interface IFarmService
{
    Task<FarmDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<FarmDto> CreateAsync(CreateFarmRequest request, CancellationToken cancellationToken = default);
}
