using System.Diagnostics.CodeAnalysis;
using Application.Api;
using Application.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using Tools.Exceptions.Errors;

namespace Application.Features.Users.GetRoles
{
    [ExcludeFromCodeCoverage]
    [ApiController]
    [Route("user")]
    [OpenApiTag("user")]
    public class GetRolesController : ApiControllerBase
    {
        /// <summary>
        /// Get the roles
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet()]
        [Route("roles", Name = "GetRoles")]
        [OpenApiOperation("GetRoles", "Récupère tous les rôles possibles pour un utilisateur.", "")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Result<List<RoleModel>>))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(Result<Error>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(Result<Error>))]
        [ProducesResponseType(StatusCodes.Status403Forbidden, Type = typeof(Result<Error>))]
        public Task<Result<List<RoleModel>>> GetRoles(CancellationToken cancellationToken)
        {
            var query = new GetRolesQuery() { };
            return Mediator.Send(query, cancellationToken);
        }
    }
}
