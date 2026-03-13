using System.Diagnostics.CodeAnalysis;
using Application.Api;
using Application.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using Tools.Exceptions.Errors;

namespace Application.Features.Constituency.CreateConstituency
{
    [ExcludeFromCodeCoverage]
    [ApiController]
    [Route("geographic-area")]
    [OpenApiTag("geographic-area")]
    public class CreateConstituencyController : ApiControllerBase
    {
        /// <summary>
        /// Create a new geographic area
        /// </summary>
        /// <param name="command"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpPost()]
        [OpenApiOperation("CreateGeographicArea", "Enregistre une nouvelle Circonscription.", "")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(Result<long>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(Result<Error>))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(Result<Error>))]
        [ProducesResponseType(StatusCodes.Status403Forbidden, Type = typeof(Result<Error>))]
        public async Task<IActionResult> CreateGeographicAreaAsync(
            [FromBody] CreateConstituencyCommand command,
            CancellationToken token
        )
        {
            var result = await Mediator.Send(command, token);
            return new ObjectResult(result) { StatusCode = StatusCodes.Status201Created };
        }
    }
}
