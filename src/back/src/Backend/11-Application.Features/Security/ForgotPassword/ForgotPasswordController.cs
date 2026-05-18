using System.Diagnostics.CodeAnalysis;
using Application.Api;
using Application.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using Tools.Exceptions.Errors;

namespace Application.Features.Security.ForgotPassword
{
    [ExcludeFromCodeCoverage]
    [ApiController]
    [Route("auth")]
    [OpenApiTag("auth")]
    public class ForgotPasswordController : ApiControllerBase
    {
        /// <summary>
        /// Reset the password
        /// </summary>
        /// <param name="mail"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpGet("{mail}")]
        [AllowAnonymous]
        [OpenApiOperation("ForgotPassword", "Recevoir le mail de réinitialisation du mot de passe", "")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Result))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(Result<Error>))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(Result<Error>))]
        [ProducesResponseType(StatusCodes.Status403Forbidden, Type = typeof(Result<Error>))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Result<Error>))]
        public Task<Result> ForgotPassword(string mail, CancellationToken token)
        {
            var query = new ForgotPasswordQuery() { UserEmail = mail };
            return Mediator.Send(query, token);
        }
    }
}
