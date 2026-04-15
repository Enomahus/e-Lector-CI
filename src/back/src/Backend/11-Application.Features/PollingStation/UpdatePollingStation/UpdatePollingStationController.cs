using System.Diagnostics.CodeAnalysis;
using Application.Api;
using Application.Features.PollingStation.UpdatePollingStation;
using Application.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;

namespace Application.Features.PollingStation.UpdatePollingStation
{
    [ExcludeFromCodeCoverage]
    [ApiController]
    [Route("polling-station")]
    [OpenApiTag("polling-station")]
    public class UpdatePollingStationController : ApiControllerBase
    {
        /// <summary>
        /// Update an existing polling station
        /// </summary>
        /// <param name="id">The polling station ID to update</param>
        /// <param name="command">The update data</param>
        /// <param name="token">Cancellation token</param>
        /// <returns>The updated polling station ID</returns>
        [HttpPut("{id}")]
        [OpenApiOperation(
            "UpdatePollingStation",
            "Met à jour un Bureau de Vote existant.",
            ""
        )]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Result<long>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(Result<object>))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(Result<object>))]
        [ProducesResponseType(StatusCodes.Status403Forbidden, Type = typeof(Result<object>))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Result<long>))]
        public async Task<IActionResult> UpdatePollingStationAsync(
            long id,
            [FromBody] UpdatePollingStationCommand command,
            CancellationToken token
        )
        {
            command.Id = id;
            var result = await Mediator.Send(command, token);

            if (result.Data == 0)
                return NotFound(result);

            return Ok(result);
        }
    }
}
