using System.Diagnostics.CodeAnalysis;
using Application.Api;
using Application.Features.Common;
using Application.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using Tools.Exceptions.Errors;

namespace Application.Features.RegistrationRequests.GetRegistrationRequestsForAdmin
{
    [ExcludeFromCodeCoverage]
    [ApiController]
    [Route("registration-requests")]
    [OpenApiTag("registration-requests")]
    public class GetRegistrationRequestsForAdminController : ApiControllerBase
    {
        [HttpPost("for-admin/get-registration-requests")]
        [OpenApiOperation(
            "GetRegistrationRequestsForAdmin",
            "Récupère toutes les demandes d'enregistrements pour les admins.",
            ""
        )]
        [ProducesResponseType(
            StatusCodes.Status200OK,
            Type = typeof(Result<PagedList<GetRegistrationRequestsForAdminResponse>>)
        )]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(Result<Error>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(Result<Error>))]
        [ProducesResponseType(StatusCodes.Status403Forbidden, Type = typeof(Result<Error>))]
        public Task<
            Result<PagedList<GetRegistrationRequestsForAdminResponse>>
        > GetRegistrationRequestsForAdminAsync(
            [FromBody] GetRegistrationRequestsForAdminQuery query,
            CancellationToken cancellationToken
        )
        {
            return Mediator.Send(query, cancellationToken);
        }
    }
}
