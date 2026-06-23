using System.Diagnostics.CodeAnalysis;
using Application.Api;
using Application.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using NSwag.Annotations;
using Tools.Exceptions.Errors;

namespace Application.Features.RegistrationRequests.UpdateRegistrationRequest
{
    [ExcludeFromCodeCoverage]
    [ApiController]
    [Route("registration-requests")]
    [OpenApiTag("registration-requests")]
    public class UpdateRegistrationRequestController : ApiControllerBase
    {
        /// <summary>
        /// Update a registrationRequest
        /// </summary>
        /// <param name="formData"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPut("{Id}")]
        [OpenApiOperation("UpdateRegistrationRequest", "Met à jour une demande.", "")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Result<Guid>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(Result<Error>))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(Result<Error>))]
        [ProducesResponseType(StatusCodes.Status403Forbidden, Type = typeof(Result<Error>))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Result<Error>))]
        public Task<Result<Guid>> UpdateRegistrationRequestAsync(
            [FromForm] UpdateRegistrationRequestFromData formData,
            CancellationToken cancellationToken
        )
        {
            var command = new UpdateRegistrationRequestCommand()
            {
                Id = formData.Id,
                RegistrationRequest = formData.GetRegistrationRequest(),
                RegistrationRequestCniOrCertificateAttachments =
                    formData.RegistrationRequestCniOrCertificateAttachments,
                Photo = formData.Photo,
            };
            return Mediator.Send(command, cancellationToken);
        }
    }
}
