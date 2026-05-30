using System.Diagnostics.CodeAnalysis;
using Application.Api;
using Application.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using Tools.Exceptions.Errors;

namespace Application.Features.Dashboard.GetDashboardStats
{
    [ExcludeFromCodeCoverage]
    [ApiController]
    [Route("dashboard")]
    [OpenApiTag("dashboard")]
    public class GetDashboardStatsController : ApiControllerBase
    {
        /// <summary>
        /// Get dashboard statistics including total
        /// </summary>
        /// <param name="documentId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost("")]
        [OpenApiOperation("GetDashboardStats", "Recupérer les statistiques du tableau de bord.", "")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Result<GetDashboardStatsResponse>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(Result<Error>))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(Result<Error>))]
        public Task<Result<GetDashboardStatsResponse>> GetDashboardStatsAsync(
            [FromBody] GetDashboardStatsQuery query,
            CancellationToken cancellationToken
        )
        {
            return Mediator.Send(query, cancellationToken);
        }
    }
}
