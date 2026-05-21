using System.Diagnostics.CodeAnalysis;
using Application.Api;
using Application.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using Tools.Exceptions.Errors;

namespace Application.Features.Citizen.GetCitizens
{
    [ExcludeFromCodeCoverage]
    [ApiController]
    [Route("citizens")]
    [OpenApiTag("citizens")]
    public class GetCititzensController : ApiControllerBase
    {
        [HttpGet("citizens")]
        [OpenApiOperation("GetCitizensAsync", "Récupère tous les départements.", "")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Result<List<GetCitizensResponse>>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(Result<Error>))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(Result<Error>))]
        [ProducesResponseType(StatusCodes.Status403Forbidden, Type = typeof(Result<Error>))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Result<Error>))]
        public Task<Result<List<GetCitizensResponse>>> GetAllCitizensAsync(
            CancellationToken cancellationToken
        )
        {
            var query = new GetCititzensQuery();

            return Mediator.Send(query, cancellationToken);
        }
    }
}
