using System.Diagnostics.CodeAnalysis;
using Application.Api;
using Application.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using Tools.Exceptions.Errors;

namespace Application.Features.GeographicArea.UpdateGeographicArea
{
    [ExcludeFromCodeCoverage]
    [ApiController]
    [Route("geographic-area")]
    [OpenApiTag("geographic-area")]
    public class UpdateGeographicAreaCommandController : ApiControllerBase
    {
        /// <summary>
        /// Update a geagraphic area.
        /// </summary>
        /// <param name="command"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpPut()]
        [OpenApiOperation("UpdateGeographicArea", "Met à jour une Circonscrption.", "")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Result<long>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(Result<Error>))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(Result<Error>))]
        [ProducesResponseType(StatusCodes.Status403Forbidden, Type = typeof(Result<Error>))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Result<Error>))]
        public async Task<Result> UpdateGeographicAreaAsync(
            [FromBody] UpdateGeographicAreaCommandQuery command,
            CancellationToken token
        )
        {
            return await Mediator.Send(command, token);
        }
    }
}
