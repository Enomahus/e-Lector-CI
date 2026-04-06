using Infrastructure.Persistence.SQLServer.Contexts;

namespace Application.Interfaces.Services;

public interface IReferenceGeneratorService
{
    Task<string> GenerateRequestReferenceAsync(WritableDbContext context, TimeProvider timeProvider, CancellationToken cancellationToken);
    Task<string> GenerateElectorNumberAsync(WritableDbContext context, long pollingStaionId, CancellationToken cancellationToken);
}
