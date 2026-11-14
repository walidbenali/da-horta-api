using DaHorta.Api.Application.Abstractions;
using DaHorta.Api.Domain.Entities;
using DaHorta.Api.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DaHorta.Api.Infrastructure.Repositories;

public sealed class FarmRepository(DaHortaDbContext db) : IFarmRepository
{
    public async Task<Farm?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await db.Farms.AsNoTracking().SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task AddAsync(Farm farm, CancellationToken cancellationToken = default)
    {
        await db.Farms.AddAsync(farm, cancellationToken);
        await db.SaveChangesAsync(cancellationToken);
    }
}
