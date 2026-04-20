using System.Diagnostics.CodeAnalysis;
using Application.Api;
using Application.Features.PollingStation.GetPollingStation;
using Application.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;

namespace Application.Features.PollingStation.Controllers
{
    [ExcludeFromCodeCoverage]
    [ApiController]
    [Route("polling-station")]
    [OpenApiTag("polling-station")]
    public class GetPollingStationController : ApiControllerBase
    {
        /// <summary>
        /// Get a polling station by ID
        /// </summary>
        /// <param name="id">The polling station ID</param>
        /// <param name="token">Cancellation token</param>
        /// <returns>The polling station details</returns>
        [HttpGet("{id}")]
        [OpenApiOperation("GetPollingStationById", "Récupère un Bureau de Vote par son ID.", "")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Result<object>))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Result<object>))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(Result<object>))]
        [ProducesResponseType(StatusCodes.Status403Forbidden, Type = typeof(Result<object>))]
        public async Task<IActionResult> GetPollingStationByIdAsync(
            long id,
            CancellationToken token
        )
        {
            var query = new GetPollingStationByIdQuery { Id = id };
            var result = await Mediator.Send(query, token);

            if (result.Data is null)
                return NotFound(result);

            return Ok(result);
        }

        /// <summary>
        /// Get all polling stations for a constituency
        /// </summary>
        /// <param name="constituencyId">The constituency ID</param>
        /// <param name="token">Cancellation token</param>
        /// <returns>List of polling stations</returns>
        [HttpGet("constituency/{constituencyId}")]
        [OpenApiOperation(
            "GetPollingStationsByConstituencyId",
            "Récupère tous les Bureaux de Vote d'une circonscription.",
            ""
        )]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Result<List<object>>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(Result<object>))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(Result<object>))]
        [ProducesResponseType(StatusCodes.Status403Forbidden, Type = typeof(Result<object>))]
        public async Task<IActionResult> GetPollingStationsByConstituencyIdAsync(
            long constituencyId,
            CancellationToken token
        )
        {
            var query = new GetPollingStationsByConstituencyIdQuery
            {
                ConstituencyId = constituencyId,
            };
            var result = await Mediator.Send(query, token);

            return Ok(result);
        }
    }
}
