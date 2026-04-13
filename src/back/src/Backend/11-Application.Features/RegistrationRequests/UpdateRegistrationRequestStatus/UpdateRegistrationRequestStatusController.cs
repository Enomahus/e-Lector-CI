using Application.Api;
using Application.Features.RegistrationRequests.UpdateRegistrationRequest;
using Application.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Tools.Exceptions.Errors;

namespace Application.Features.RegistrationRequests.UpdateRegistrationRequestStatus
{
    [ExcludeFromCodeCoverage]
    [ApiController]
    [Route("registration-requests")]
    [OpenApiTag("registration-requests")]
    public class UpdateRegistrationRequestStatusController : ApiControllerBase
    {
        /// <summary>
        /// Update a registrationRequest status
        /// </summary>
        /// <param name="id"></param>
        /// <param name="command"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPut("{Id}/status")]
        [OpenApiOperation("UpdateRegistrationRequestStatus", "Met à jour le status d'une demande.", "")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Result<UpdateRegistrationRequestStatusResponse>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(Result<Error>))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(Result<Error>))]
        [ProducesResponseType(StatusCodes.Status403Forbidden, Type = typeof(Result<Error>))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Result<Error>))]
        public Task<Result<UpdateRegistrationRequestStatusResponse>> UpdateRegistrationRequestStatusAsync(
            [FromRoute] Guid id,
            [FromBody] UpdateRegistrationRequestStatusCommand command,
            CancellationToken cancellationToken
        )
        {
            command.RegistrationRequestId = id;
            return Mediator.Send(command, cancellationToken);
        }
    }
}
