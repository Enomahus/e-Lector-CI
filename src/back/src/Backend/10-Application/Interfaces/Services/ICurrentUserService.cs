namespace Application.Interfaces.Services;

public interface ICurrentUserService
{
    Guid? UserId { get; }
    string? UserEmail { get; }
    string? ClientIp { get; }
    string? LanguageCode { get; }
    Guid? ImpersonatorId { get; }
    string? ImpersonatorEmail { get; }
    Task<string?> GetTokenAsync(CancellationToken token = default);
}
