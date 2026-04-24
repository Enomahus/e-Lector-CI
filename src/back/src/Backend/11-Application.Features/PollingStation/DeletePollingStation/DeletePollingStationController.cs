using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Application.Api;
using Application.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using Tools.Exceptions.Errors;

namespace Application.Features.PollingStation.DeletePollingStation
{
    [ExcludeFromCodeCoverage]
    [ApiController]
    [Route("polling-station")]
    [OpenApiTag("polling-station")]
    public class DeletePollingStationController : ApiControllerBase
    {
        /// <summary>
        /// Delete a polling station
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [OpenApiOperation("DeletePollingStation", "Supprime une entité.", "")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Result))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(Result<Error>))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(Result<Error>))]
        public Task<Result> DeletePollingStationAsync(long id, CancellationToken cancellationToken)
        {
            var command = new DeletePollingStationCommand(id);
            return Mediator.Send(command, cancellationToken);
        }
    }
}
