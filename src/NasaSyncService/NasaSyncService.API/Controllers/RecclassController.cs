using MediatR;
using Microsoft.AspNetCore.Mvc;
using NasaSyncService.Application.Features.Recclasses.GetRecclasses;

namespace NasaSyncService.API.Controllers
{
    [ApiController]
    [Route("api/v1/recclasses")]
    public class RecclassController(
        IMediator mediator) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetRecclasses(CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GetRecclassesQuery { }, cancellationToken);

            return Ok(result);
        }
    }
}
