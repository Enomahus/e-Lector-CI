using Application.Exceptions.Auth;
using Application.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using System.Text.Json;
using Tools.Exceptions.Errors;

namespace Web.Common.Handlers;

public class CustomAuthorizationResultHandler(JsonSerializerOptions jsonOptions) : IAuthorizationMiddlewareResultHandler
{

    private readonly AuthorizationMiddlewareResultHandler _defaultHandler = new();

    public async Task HandleAsync(RequestDelegate next, HttpContext context, AuthorizationPolicy policy, PolicyAuthorizationResult authorizeResult)
    {
        if (authorizeResult.Forbidden)
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            var ex = new UserAccessException();
            await context.Response.WriteAsJsonAsync(Result<Error>.From(ex.ToError()), jsonOptions);
        }
        else
        {
            await _defaultHandler.HandleAsync(next, context, policy, authorizeResult);
        }
    }
}
