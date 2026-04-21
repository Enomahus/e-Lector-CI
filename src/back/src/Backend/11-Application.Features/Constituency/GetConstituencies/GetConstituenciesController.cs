using Application.Api;
using Application.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using System.Diagnostics.CodeAnalysis;
using Tools.Exceptions.Errors;

namespace Application.Features.Constituency.GetConstituencies
{
    [ExcludeFromCodeCoverage]
    [ApiController]
    [Route("constituencies")]
    [OpenApiTag("constituencies")]
    public class GetConstituenciesController : ApiControllerBase
    {
        /// <summary>
        /// Get constituencies 
        /// </summary>
        /// <param name="query"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost("get-constituencies")]
        [OpenApiOperation("GetConstituencies", "Récupère les circonscription.", "")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Result<IEnumerable<GetConstituenciesResponse>>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(Result<Error>))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(Result<Error>))]
        [ProducesResponseType(StatusCodes.Status403Forbidden, Type = typeof(Result<Error>))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Result<Error>))]
        public Task<Result<IEnumerable<GetConstituenciesResponse>>> GetConstituenciesAsync(
            [FromBody] GetConstituenciesQuery query,
            CancellationToken cancellationToken
        )
        {
            return Mediator.Send(query, cancellationToken);
        }
    }
}
