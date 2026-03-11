using Application.Interfaces.Services;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;

namespace Web.Services
{
    public class CurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentUserService

    {
        public Guid? UserId => Guid.TryParse(
                httpContextAccessor
                    .HttpContext?.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)
                    ?.Value ?? "",
                out var parsedId
            )
                ? parsedId
                : null;

        public string? UserEmail => httpContextAccessor
                .HttpContext?.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)
                ?.Value;

        public string? ClientIp => httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString() ?? "";

        public string? LanguageCode => httpContextAccessor
                .HttpContext?.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Locality)
                ?.Value;

        public Task<string?> GetTokenAsync(CancellationToken token = default)
        {
            return httpContextAccessor.HttpContext?.GetTokenAsync("access_token")
                ?? Task.FromResult<string?>(null);
        }
    }
}
