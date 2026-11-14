using DaHorta.Api.Domain.Entities;

namespace DaHorta.Api.Application.Abstractions;

public interface IFarmRepository
{
    Task<Farm?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(Farm farm, CancellationToken cancellationToken = default);
}
