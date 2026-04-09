using Application.Api;
using Application.Features.Security.Common;
using Application.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Tools.Exceptions.Errors;

namespace Application.Features.Security.AuthenticateGoogle
{
    [ExcludeFromCodeCoverage]
    [ApiController]
    [Route("auth")]
    [OpenApiTag("auth")]
    public class AuthenticateGoogleController : ApiControllerBase
    {
        /// <summary>
        /// Authenticate a google user with an authorization code
        /// </summary>
        /// <param name="authCode"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost("token-google")]
        [AllowAnonymous]
        [OpenApiOperation("AuthenticateGoogle", "Authentifie un utilisateur Google.", "")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Result<TokenResponse>))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(Result<Error>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(Result<Error>))]
        public Task<Result<TokenResponse>> AuthenticateGoogle(
            [FromQuery] string authCode,
            CancellationToken cancellationToken
        )
        {
            var command = new AuthenticateGoogleCommand() { AuthCode = authCode };
            return Mediator.Send(command, cancellationToken);
        }
    }
}
