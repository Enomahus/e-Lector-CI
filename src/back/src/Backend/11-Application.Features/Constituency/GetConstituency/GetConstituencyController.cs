using Application.Api;
using Application.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using System.Diagnostics.CodeAnalysis;
using Tools.Exceptions.Errors;

namespace Application.Features.Constituency.GetConstituency
{
    [ExcludeFromCodeCoverage]
    [ApiController]
    [Route("constituencies")]
    [OpenApiTag("constituencies")]
    public class GetConstituencyController: ApiControllerBase
    {
        /// <summary>
        /// Get constituency by Id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [OpenApiOperation("GetConstituency", "Récupère une circonscription par son identifiant.", "")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Result<GetConstituencyResponse>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(Result<Error>))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(Result<Error>))]
        [ProducesResponseType(StatusCodes.Status403Forbidden, Type = typeof(Result<Error>))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Result<Error>))]
        public Task<Result<GetConstituencyResponse>> GetConstituencyAsync(
            long id,
            CancellationToken cancellationToken
        )
        {
            var query = new GetConstituencyQuery(id);
            return Mediator.Send(query, cancellationToken);
        }
    }
}
