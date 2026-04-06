using Application.Common.Enums;
using Application.Interfaces.Services;
using Infrastructure.Persistence.Entities;
using Infrastructure.Persistence.SQLServer.Contexts;
using Microsoft.AspNetCore.Http;

namespace Application.Features.RegistrationRequests.Common
{
    public class RegistrationRequestService(
        WritableDbContext context,
        IFileService fileService
    )
    {
        private async Task UpdateRegistrationRequestDocuments(
            RegistrationRequestCommandBase command,
            RegistrationRequestDao registrationRequestDao,
            CancellationToken cancellationToken
        ) 
        {
            //Remove old document
            await RemoveObsoleteDocuments(
                [.. command.RegistrationRequest?.CertificateOfNationalityDocumentIds ?? []],
                RegistrationRequestDocumentType.CertificateOfNationality,
                registrationRequestDao, cancellationToken
            );

            await RemoveObsoleteDocuments(
                [.. command.RegistrationRequest?.IdentityDocumentIds ?? []],
                RegistrationRequestDocumentType.IdentityDocument,
                registrationRequestDao, cancellationToken
            );

            //Upload documents
            await UploadRegistrationRequestDocumentsByType(
                [.. command.RegistrationRequestCertificateAttachments],
                RegistrationRequestDocumentType.CertificateOfNationality,
                registrationRequestDao, cancellationToken
            );

            await UploadRegistrationRequestDocumentsByType(
                [.. command.RegistrationRequestCniAttachments],
                RegistrationRequestDocumentType.IdentityDocument,
                registrationRequestDao, cancellationToken
            );
        }

        private async Task UploadRegistrationRequestDocumentsByType(
            List<IFormFile> attachements,
            RegistrationRequestDocumentType documentType,
            RegistrationRequestDao existingRegistrationRequest,
            CancellationToken cancellationToken
        )
        {
            var existingDocumentsForType = existingRegistrationRequest
                .RegistrationRequestDocuments
                ?.Where(doc => doc.RegistrationRequestDocumentType == documentType)
                .ToList() ?? [];

            var newDocuments = new List<RegistrationRequestDocumentDao>();

            foreach (var file in attachements)
            {
                var existingDocument = existingDocumentsForType.FirstOrDefault(doc =>
                    string.Equals(doc.Document.FileName, file.FileName)                    
                );

                Guid? existingDocumentId = existingDocument?.DocumentId;

                using var stream = file.OpenReadStream();

                var documentId = await fileService.UploadFileNoTransactionAsync(
                    stream, file.FileName, file.ContentType, context, cancellationToken, existingDocumentId
                );

                if(existingDocument is null)
                {
                    var newDocument = new RegistrationRequestDocumentDao
                    {
                        RegistrationRequestId = existingRegistrationRequest.Id,
                        DocumentId = documentId,
                        RegistrationRequestDocumentType = documentType,
                    };

                    newDocuments.Add(newDocument);
                }
            }

            context.RegistrationRequestDocuments.AddRange(newDocuments);
            await context.SaveChangesAsync(cancellationToken);
        }

        protected async Task RemoveObsoleteDocuments(
            List<Guid> newAttachmentsIds,
            RegistrationRequestDocumentType documentType,
            RegistrationRequestDao? existingRegistrationRequest,
            CancellationToken cancellationToken = default
        )
        {
            if (existingRegistrationRequest == null) return;

            var obsoleteDocuments = existingRegistrationRequest.
                RegistrationRequestDocuments.Where(doc =>
                    doc.RegistrationRequestDocumentType == documentType
                    && !newAttachmentsIds.Contains(doc.Id)
                )
                .ToList();

            foreach (var document in obsoleteDocuments)
            {
                await fileService.DeleteFileAsync(document.DocumentId, context, cancellationToken);
            }
        }
    }
}
