using System.Diagnostics.CodeAnalysis;
using Application.Api;
using Application.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using Tools.Exceptions.Errors;

namespace Application.Features.PollingStation.CreatePollingStation
{
    [ExcludeFromCodeCoverage]
    [ApiController]
    [Route("polling-station")]
    [OpenApiTag("polling-station")]
    public class CreatePollingStationController : ApiControllerBase
    {
        /// <summary>
        /// Create a new polling station
        /// </summary>
        /// <param name="command"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpPost()]
        [OpenApiOperation("CreatePollingStation", "Enregistre un nouveau Bureau de Vote.", "")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(Result<long>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(Result<Error>))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(Result<Error>))]
        [ProducesResponseType(StatusCodes.Status403Forbidden, Type = typeof(Result<Error>))]
        public async Task<IActionResult> CreatePollingStationAsync(
            [FromBody] CreatePollingStationCommand command,
            CancellationToken token
        )
        {
            var result = await Mediator.Send(command, token);
            return new ObjectResult(result) { StatusCode = StatusCodes.Status201Created };
        }
    }
}
