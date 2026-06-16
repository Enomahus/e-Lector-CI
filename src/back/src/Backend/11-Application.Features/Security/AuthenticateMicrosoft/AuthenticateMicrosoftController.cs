using System.Diagnostics.CodeAnalysis;
using Application.Api;
using Application.Features.Security.Common;
using Application.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using Tools.Exceptions.Errors;

namespace Application.Features.Security.AuthenticateMicrosoft
{
    [ExcludeFromCodeCoverage]
    [ApiController]
    [Route("auth")]
    [OpenApiTag("auth")]
    public class AuthenticateMicrosoftController : ApiControllerBase
    {
        /// <summary>
        /// Authenticate a Microsoft user with an authorization code
        /// </summary>
        /// <param name="authCode"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost("token-microsoft")]
        [AllowAnonymous]
        [OpenApiOperation("AuthenticateMicrosoft", "Authentifie un utilisateur Microsoft.", "")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Result<TokenResponse>))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(Result<Error>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(Result<Error>))]
        public Task<Result<TokenResponse>> AuthenticateMicrosoft(
            [FromQuery] string authCode,
            CancellationToken cancellationToken
        )
        {
            var command = new AuthenticateMicrosoftCommand() { AuthCode = authCode };
            return Mediator.Send(command, cancellationToken);
        }
    }
}
