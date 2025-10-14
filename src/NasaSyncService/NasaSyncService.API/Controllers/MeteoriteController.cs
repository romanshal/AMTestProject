using MediatR;
using Microsoft.AspNetCore.Mvc;
using NasaSyncService.Application.Features.Meteorites.GetGroupedMeteorites;

namespace NasaSyncService.API.Controllers
{
    [ApiController]
    [Route("api/v1/meteorites")]
    public class MeteoriteController(
        IMediator mediator) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetGroupedAsync(
            [FromQuery] int? yearFrom,
            [FromQuery] int? yearTo,
            [FromQuery] string? name,
            [FromQuery] Guid? recclass,
            [FromQuery] string sortBy,
            [FromQuery] string sortOrder,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GetGroupedMeteoritesQuery(
                yearFrom,
                yearTo,
                recclass,
                name,
                sortBy,
                sortOrder == "desc"), cancellationToken);

            return Ok(result);
        }
    }
}
