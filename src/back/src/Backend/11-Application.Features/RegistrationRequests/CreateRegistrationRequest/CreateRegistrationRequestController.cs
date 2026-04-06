using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using Application.Api;
using Application.Features.RegistrationRequests.Common;
using Application.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using NSwag.Annotations;
using Tools.Exceptions.Errors;

namespace Application.Features.RegistrationRequests.CreateRegistrationRequest
{
    [ExcludeFromCodeCoverage]
    [ApiController]
    [Route("registration-requests")]
    [OpenApiTag("registration-requests")]
    public class CreateRegistrationRequestController(IOptions<JsonOptions> jsonOptions) : ApiControllerBase
    {

        /// <summary>
        /// Create a new registration request
        /// </summary>
        /// <param name="formData"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpPost()]
        [OpenApiOperation("CreateRegistrationRequest", "Enregistre une nouvelle demande d'enrôlement.", "")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(Result<Guid>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(Result<Error>))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(Result<Error>))]
        [ProducesResponseType(StatusCodes.Status403Forbidden, Type = typeof(Result<Error>))]
        public async Task<IActionResult> CreateRegistrationRequestAsync(
            [FromForm] CreateRegistrationRequestFromData formData,
            CancellationToken token
        )
        {
            // Sécurité : on s'assure que les options de sérialisation sont bien récupérées
            var serializerOptions = jsonOptions.Value.JsonSerializerOptions;

            var command = new CreateRegistrationRequestCommand()
            {
                RegistrationRequest = !string.IsNullOrWhiteSpace(formData.RegistrationRequestJson)
                ? JsonSerializer.Deserialize<RegistrationRequestModel>(
                    formData.RegistrationRequestJson,
                    serializerOptions
                ) : null,
                RegistrationRequestCertificateAttachments = formData.RegistrationRequestCertificateAttachments,
                RegistrationRequestCniAttachments = formData.RegistrationRequestCniAttachments,
            };

            var result = await Mediator.Send(command, token);

            return StatusCode(StatusCodes.Status201Created, result);
        }
    }
}
