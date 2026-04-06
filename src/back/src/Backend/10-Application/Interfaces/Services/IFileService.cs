using Infrastructure.Persistence.SQLServer.Contexts;

namespace Application.Interfaces.Services
{
    public interface IFileService
    {
        Task DeleteFileAsync(Guid documentId, WritableDbContext context, CancellationToken cancellationToken);

        Task<Stream> GetFileDownloadStreamAsync(Guid documentId, CancellationToken cancellationToken);
        Task<Guid> UploadFileNoTransactionAsync(Stream stream,
            string fileName,
            string contentType,
            WritableDbContext context,
            CancellationToken cancellationToken,
            Guid? existingDocumentId = null
        );

        Task<Guid> UploadFileAsync(
            Stream stream,
            string fileName,
            string contentType,
            WritableDbContext context,
            CancellationToken cancellationToken,
            Guid? existingDocumentId = null
        );

        //Task<DocumentDao> UploadFileAsync(WritableDbContext context, IFormFile file, CancellationToken cancellationToken);
        //Task<(byte[] content, string contentType, string fileName)> DownloadFileAsync(Guid fileId, WritableDbContext context, CancellationToken token);
    }
}
