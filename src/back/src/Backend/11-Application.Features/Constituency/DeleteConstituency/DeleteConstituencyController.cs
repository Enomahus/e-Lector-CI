using Application.Api;
using Application.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using System.Diagnostics.CodeAnalysis;
using Tools.Exceptions.Errors;

namespace Application.Features.Constituency.DeleteConstituency
{
    [ExcludeFromCodeCoverage]
    [ApiController]
    [Route("constituencies")]
    [OpenApiTag("constituencies")]
    public class DeleteConstituencyController: ApiControllerBase
    {
        /// <summary>
        /// Delete a constituency
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [OpenApiOperation("DeleteConstituency", "Supprime une circonscription.", "")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Result))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(Result<Error>))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(Result<Error>))]
        public Task<Result> DeleteConstituencyAsync(long id, CancellationToken cancellationToken)
        {
            var command = new DeleteConstituencyCommand(id);
            return Mediator.Send(command, cancellationToken);
        }
    }
}
