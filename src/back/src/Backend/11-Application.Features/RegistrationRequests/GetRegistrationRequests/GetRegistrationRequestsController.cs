using System.Diagnostics.CodeAnalysis;
using Application.Api;
using Application.Features.Common;
using Application.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using Tools.Exceptions.Errors;

namespace Application.Features.RegistrationRequests.GetRegistrationRequests
{
    [ExcludeFromCodeCoverage]
    [ApiController]
    [Route("registration-requests")]
    [OpenApiTag("registration-requests")]
    public class GetRegistrationRequestsController : ApiControllerBase
    {
        [HttpPost("get-registration-requests")]
        [OpenApiOperation(
            "GetCertificateRequests",
            "Récupère toutes les demandes d'attestations de l'utilisateur.",
            ""
        )]
        [ProducesResponseType(
            StatusCodes.Status200OK,
            Type = typeof(Result<PagedList<GetRegistrationRequestsResponse>>)
        )]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(Result<Error>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(Result<Error>))]
        [ProducesResponseType(StatusCodes.Status403Forbidden, Type = typeof(Result<Error>))]
        public Task<Result<PagedList<GetRegistrationRequestsResponse>>> GetRegistrationRequestsAsync(
            [FromBody] GetRegistrationRequestsQuery query,
            CancellationToken cancellationToken
        )
        {
            return Mediator.Send(query, cancellationToken);
        }
    }
}
