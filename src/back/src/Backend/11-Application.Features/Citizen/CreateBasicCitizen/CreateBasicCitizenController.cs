using System.Diagnostics.CodeAnalysis;
using Application.Api;
using Application.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using Tools.Exceptions.Errors;

namespace Application.Features.Citizen.CreateBasicCitizen
{
    [ExcludeFromCodeCoverage]
    [ApiController]
    [Route("citizens")]
    [OpenApiTag("citizens")]
    public class CreateBasicCitizenController : ApiControllerBase
    {
        /// <summary>
        /// Create a basic citizen
        /// </summary>
        /// <param name="command"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpPost()]
        [OpenApiOperation("CreateBasicCitizen", "Enregistre une nouveau citoyen basic.", "")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(Result<Guid>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(Result<Error>))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(Result<Error>))]
        [ProducesResponseType(StatusCodes.Status403Forbidden, Type = typeof(Result<Error>))]
        public async Task<IActionResult> CreateBasicCitizenAsync(
            [FromBody] CreateBasicCitizenCommand command,
            CancellationToken token
        )
        {
            var result = await Mediator.Send(command, token);
            return new ObjectResult(result) { StatusCode = StatusCodes.Status201Created };
        }
    }
}
