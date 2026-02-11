using Application.Interfaces.Services;

namespace Infrastructure.Services;

public class DataIntegrationService : IDataIntegrationService
{
    public Task<bool> ImportFileAsync(string filePath, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<bool> ImportFileAsync(Stream stream, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
