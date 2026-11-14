using DaHorta.Api.Application.Abstractions;
using DaHorta.Api.Domain.Entities;
using DaHorta.Api.Domain.Enums;
using DaHorta.Api.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DaHorta.Api.Infrastructure.Repositories;

public sealed class ListingRepository(DaHortaDbContext db) : IListingRepository
{
    public async Task<List<Listing>> GetActiveListingsAsync(CancellationToken cancellationToken = default)
    {
        return await db.Listings
            .AsNoTracking()
            .Include(x => x.Farm)
            .Where(x => x.Status == ListingStatus.Active && x.ExpiresAt > DateTimeOffset.UtcNow)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<Listing?> GetByIdAsync(Guid id, bool includeFarm = true, bool includeReservations = false, CancellationToken cancellationToken = default)
    {
        IQueryable<Listing> query = db.Listings.AsNoTracking();

        if (includeFarm)
        {
            query = query.Include(x => x.Farm);
        }

        if (includeReservations)
        {
            query = query.Include(x => x.Reservations);
        }

        return await query.SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task AddAsync(Listing listing, CancellationToken cancellationToken = default)
    {
        await db.Listings.AddAsync(listing, cancellationToken);
        await db.SaveChangesAsync(cancellationToken);
    }

    public async Task AddReservationAsync(Reservation reservation, CancellationToken cancellationToken = default)
    {
        await db.Reservations.AddAsync(reservation, cancellationToken);
        await db.SaveChangesAsync(cancellationToken);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await db.SaveChangesAsync(cancellationToken);
    }
}
