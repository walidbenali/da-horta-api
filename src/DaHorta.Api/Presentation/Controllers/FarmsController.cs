using DaHorta.Api.Application.Abstractions;
using DaHorta.Api.Application.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace DaHorta.Api.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[Tags("Farms")]
public sealed class FarmsController(IFarmService farmService) : ControllerBase
{
    /// <summary>
    /// Gets a farm profile by identifier.
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(FarmDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<FarmDto>> GetFarmById(Guid id, CancellationToken cancellationToken)
    {
        var farm = await farmService.GetByIdAsync(id, cancellationToken);
        return farm is null ? NotFound() : Ok(farm);
    }

    /// <summary>
    /// Registers a new farm with the required geo location and contact details.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(FarmDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<FarmDto>> CreateFarm([FromBody] CreateFarmRequest request, CancellationToken cancellationToken)
    {
        var farm = await farmService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetFarmById), new { id = farm.Id }, farm);
    }
}
