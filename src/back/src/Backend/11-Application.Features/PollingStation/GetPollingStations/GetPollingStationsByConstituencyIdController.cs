using Application.Api;
using Application.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using System.Diagnostics.CodeAnalysis;
using Tools.Exceptions.Errors;

namespace Application.Features.PollingStation.GetPollingStations
{
    [ExcludeFromCodeCoverage]
    [ApiController]
    [Route("polling-station")]
    [OpenApiTag("polling-station")]
    public class GetPollingStationsByConstituencyIdController: ApiControllerBase
    {
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
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Result<List<GetPollingStationsByConstituencyIdResponse>>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(Result<Error>))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(Result<Error>))]
        [ProducesResponseType(StatusCodes.Status403Forbidden, Type = typeof(Result<Error>))]
        public async Task<Result<List<GetPollingStationsByConstituencyIdResponse>>> GetPollingStationsByConstituencyIdAsync(
            long constituencyId,
            CancellationToken token
        )
        {
            var query = new GetPollingStationsByConstituencyIdQuery(constituencyId);
            
            return await Mediator.Send(query, token);
                        
        }
    }
}
