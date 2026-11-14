using DaHorta.Api.Application.Abstractions;
using DaHorta.Api.Application.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace DaHorta.Api.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[Tags("Listings")]
public sealed class ListingsController(IListingService listingService) : ControllerBase
{
    /// <summary>
    /// Lists active listings near a coordinate or within a maximum distance.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ListingSummaryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IEnumerable<ListingSummaryDto>>> GetListings(
        [FromQuery] double? latitude,
        [FromQuery] double? longitude,
        [FromQuery] double? maxDistanceKm,
        CancellationToken cancellationToken)
    {
        var listings = await listingService.GetListingsAsync(latitude, longitude, maxDistanceKm, cancellationToken);
        return Ok(listings);
    }

    /// <summary>
    /// Gets the full detail of a listing, optionally including proximity data for a consumer coordinate.
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ListingDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ListingDetailDto>> GetListingById(
        Guid id,
        [FromQuery] double? latitude,
        [FromQuery] double? longitude,
        CancellationToken cancellationToken)
    {
        var listing = await listingService.GetByIdAsync(id, latitude, longitude, cancellationToken);
        return listing is null ? NotFound() : Ok(listing);
    }

    /// <summary>
    /// Creates a new farm listing for fresh produce available for collection.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ListingDetailDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ListingDetailDto>> CreateListing([FromBody] CreateListingRequest request, CancellationToken cancellationToken)
    {
        var listing = await listingService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetListingById), new { id = listing.Id }, listing);
    }

    /// <summary>
    /// Reserves a quantity of produce from a listing for later collection.
    /// </summary>
    [HttpPost("{id:guid}/reservations")]
    [ProducesResponseType(typeof(ReservationDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ReservationDto>> ReserveListing(
        Guid id,
        [FromBody] CreateReservationRequest request,
        CancellationToken cancellationToken)
    {
        var reservation = await listingService.ReserveAsync(id, request, cancellationToken);
        return CreatedAtAction(nameof(GetListingById), new { id }, reservation);
    }
}
