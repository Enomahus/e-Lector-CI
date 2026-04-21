using System.Diagnostics.CodeAnalysis;
using Application.Api;
using Application.Features.Common.PollingStation;
using Application.Features.PollingStation.GetPollingStation;
using Application.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using Tools.Exceptions.Errors;

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
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Result<PollingStationModel>))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Result<Error>))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(Result<Error>))]
        [ProducesResponseType(StatusCodes.Status403Forbidden, Type = typeof(Result<Error>))]
        public async Task<Result<PollingStationModel>> GetPollingStationByIdAsync(
            long id,
            CancellationToken token
        )
        {
            var query = new GetPollingStationByIdQuery { Id = id };
            return await Mediator.Send(query, token);            
            
        }        
    }
}
