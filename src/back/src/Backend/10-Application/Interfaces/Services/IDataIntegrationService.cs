namespace Application.Interfaces.Services;

public interface IDataIntegrationService
{
    Task<bool> ImportFileAsync(string filePath, CancellationToken cancellationToken);
    Task<bool> ImportFileAsync(Stream stream, CancellationToken cancellationToken);
}
