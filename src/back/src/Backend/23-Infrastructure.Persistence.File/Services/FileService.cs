using Application.Exceptions;
using Application.Interfaces.Services;
using Infrastructure.Persistence.Entities;
using Infrastructure.Persistence.File.Configurations;
using Infrastructure.Persistence.SQLServer.Contexts;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.IO;


namespace Infrastructure.Persistence.File.Services
{
    public class FileService(
        IOptions<StorageConfiguration> storageConfig,
        ILogger<FileService> logger
    ) : IFileService
    {

        //private readonly string _storagePath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");

        //public FileService(string storagePath)
        //{
        //    _storagePath = storagePath;

        //    if(!Directory.Exists(_storagePath))
        //        Directory.CreateDirectory(_storagePath);
        //}


        //#region Local file service
        //public async Task<(byte[] content, string contentType, string fileName)> DownloadFileAsync(Guid fileId, WritableDbContext context, CancellationToken token)
        //{
        //    var record = await context.Documents.FirstOrDefaultAsync(d => d.Id == fileId, token)
        //        ?? throw new NotFoundException(nameof(DocumentDao), fileId);

        //    var path = Path.Combine(_storagePath, record.StoredName);
        //    var bytes = await System.IO.File.ReadAllBytesAsync(path,token);


        //    return (bytes, record.ContentType, record.FileName);
        //}

        //public async Task<DocumentDao> UploadFileAsync(WritableDbContext context, IFormFile file, CancellationToken cancellationToken)
        //{
        //    ArgumentNullException.ThrowIfNull(file);

        //    var record = new DocumentDao
        //    {
        //        FileName = file.FileName,
        //        StoredName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName),
        //        ContentType = file.ContentType,
        //        FileSize = file.Length,
        //    };

        //    var fullPath = Path.Combine(_storagePath, record.StoredName);

        //    using (var stream = new FileStream(fullPath, FileMode.Create))
        //    {
        //        await file.CopyToAsync(stream, cancellationToken);
        //    }

        //    context.Documents.Add(record);
        //    await context.SaveChangesAsync(cancellationToken);

        //    return record;
        //}

        //#endregion

        private readonly string _basePath = storageConfig.Value.RootPath;

        
        

        public Task<Stream> GetFileDownloadStreamAsync(Guid documentId, CancellationToken cancellationToken)
        {
            string path = GetPhysicalPath(documentId);

            if (!System.IO.File.Exists(path))
            {
                throw new StorageException($"File with ID {documentId} not found on disk.");
            }

            try
            {
                // On ouvre le stream sans tout charger en RAM (crucial pour le lead dev)
                Stream stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, useAsync: true);
                return Task.FromResult(stream);
            }
            catch (Exception ex)
            {
                throw new StorageException("Error while opening file stream.", ex);
            }
        }

        public async Task<Guid> UploadFileAsync(
            Stream stream, 
            string fileName, 
            string contentType, 
            WritableDbContext context, 
            CancellationToken cancellationToken, 
            Guid? existingDocumentId = null
        )
        {

            var documentId = Guid.Empty;
            var strategy = context.Database.CreateExecutionStrategy();
            await strategy.ExecuteInTransactionAsync(
                async () =>
                {
                    documentId = await UploadFileNoTransactionAsync(
                        stream, fileName, contentType, context, cancellationToken, existingDocumentId
                    );
                },
                () => Task.FromResult(true)
            );

            return documentId;
        }              

        public async Task<Guid> UploadFileNoTransactionAsync(
            Stream stream, 
            string fileName, 
            string contentType, 
            WritableDbContext context, 
            CancellationToken cancellationToken, 
            Guid? existingDocumentId = null
        )
        {
            DocumentDao document;
            if (existingDocumentId is null)
            {
                document = new DocumentDao
                {
                    FileName = fileName,
                    ContentType = contentType,
                    FileSize = stream.Length
                };
                context.Add(document);
            }
            else
            {
                document = await context.Documents.FirstOrDefaultAsync(d => d.Id == existingDocumentId, cancellationToken)
                           ?? throw new NotFoundException(nameof(DocumentDao), existingDocumentId);

                document.FileName = fileName;
                document.ContentType = contentType;
                document.FileSize = stream.Length;
            }

            // On sauvegarde d'abord en DB pour avoir l'ID définitif
            await context.SaveChangesAsync(cancellationToken);

            try
            {
                string filePath = GetPhysicalPath(document.Id);
                string? directory = Path.GetDirectoryName(filePath);

                if (directory != null && !Directory.Exists(directory))
                    Directory.CreateDirectory(directory);

                // On utilise FileStream avec Bufferisation pour la performance
                await using var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None, 4096, useAsync: true);
                stream.Position = 0; // Reset si le stream a été lu
                await stream.CopyToAsync(fileStream, cancellationToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Erreur lors de l'écriture physique du fichier {Id}", document.Id);
                throw new StorageException("Error while uploading file to local storage.", ex);
            }

            return document.Id;
        }

        public async Task DeleteFileAsync(Guid documentId, WritableDbContext context, CancellationToken cancellationToken)
        {
            var document =
                await context.Documents.FirstOrDefaultAsync(d => d.Id == documentId, cancellationToken)
                ?? throw new NotFoundException(nameof(DocumentDao), documentId);

            string path = GetPhysicalPath(document.Id);

            try
            {
                if (System.IO.File.Exists(path))
                {
                    System.IO.File.Delete(path);
                }
            }
            catch (IOException ex)
            {
                throw new StorageException("Error while deleting physical file", ex);
            }

            context.Documents.Remove(document);
            await context.SaveChangesAsync(cancellationToken);
        }

        /// <summary>
        /// Génère un chemin structuré pour éviter d'avoir trop de fichiers dans un seul dossier.
        /// Exemple : root/ab/cd/guid-complet.dat
        /// </summary>
        private string GetPhysicalPath(Guid documentId)
        {
            string idStr = documentId.ToString();
            // Sharding : on utilise les 4 premiers caractères pour créer des sous-dossiers
            string subFolder1 = idStr[..2];
            string subFolder2 = idStr.Substring(2, 2);

            //return Path.Combine(_basePath, subFolder1, subFolder2, idStr + ".dat");
            return Path.Combine(_basePath, subFolder1, subFolder2, $"{idStr}.dat");
        }
    }
}
