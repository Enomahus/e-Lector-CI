using Application.Common.Enums;
using Application.Exceptions;
using Application.Exceptions.Auth;
using Application.Interfaces.Services;
using Infrastructure.Persistence.Entities;
using Infrastructure.Persistence.SQLServer.Contexts;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Pcea.Core.Net.Authorization.Application.Interfaces.Services;

namespace Application.Features.RegistrationRequests.Common
{
    public class RegistrationRequestService(
        ICurrentUserService currentUserService,
        ICurrentUserPermissionsProvider currentUserPermissions,
        WritableDbContext context,
        IFileService fileService
    )
    {
        public async Task<RegistrationRequestDao> PrepareDaoForUpdate(
            RegistrationRequestCommandBase command,
            CancellationToken cancellationToken
        )
        {
            var registrationRequest =
                await context
                    .RegistrationRequests.Include(r => r.RegistrationRequestDocuments)
                        .ThenInclude(r => r.Document)
                    .Include(r => r.Citizen)
                        .ThenInclude(c => c.Father)
                    .Include(r => r.Citizen)
                        .ThenInclude(c => c.Mother)
                    .Include(r => r.Constituency)
                    .FirstOrDefaultAsync(r => r.Id == command.RegistrationRequest!.Id, cancellationToken)
                ?? throw new NotFoundException(
                    nameof(RegistrationRequestDao),
                    command.RegistrationRequest!.Id
                );

            var currentUserId = currentUserService.UserId;
            var currentUser =
                await context
                    .Users.Include(u => u.UserConstituencies)
                        .ThenInclude(c => c.Constituency)
                    .FirstOrDefaultAsync(u => u.Id == currentUserId, cancellationToken)
                ?? throw new NotFoundException(nameof(UserDao), currentUserId);

            var permissions = await currentUserPermissions.GetCurrentUserPermissionsAsync(cancellationToken);
            var userIsSuperAdmin = permissions.Contains(AppPermission.SuperAdmin.ToString());
            long constituencyId = command.RegistrationRequest!.ConstituencyId!.Value;

            if (
                !userIsSuperAdmin
                    && currentUser.UserConstituencies.Any(u =>
                        u.ConstituencyId != registrationRequest.ConstituencyId
                    )
                || currentUser.Id != registrationRequest.AuthorId
            )
            {
                throw new UserAccessException();
            }

            await UpdateRegistrationRequestDocuments(command, registrationRequest, cancellationToken);

            command.RegistrationRequest!.ToDao(constituencyId);

            registrationRequest.LastUpdaterId = currentUser.Id;

            return registrationRequest;
        }

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
                registrationRequestDao,
                cancellationToken
            );

            await RemoveObsoleteDocuments(
                [.. command.RegistrationRequest?.IdentityDocumentIds ?? []],
                RegistrationRequestDocumentType.IdentityDocument,
                registrationRequestDao,
                cancellationToken
            );
            await RemoveObsoleteDocuments(
                [.. command.RegistrationRequest?.PhotoIds ?? []],
                RegistrationRequestDocumentType.Photo,
                registrationRequestDao,
                cancellationToken
            );

            //Upload documents
            await UploadRegistrationRequestDocumentsByType(
                [.. command.RegistrationRequestCertificateAttachments],
                RegistrationRequestDocumentType.CertificateOfNationality,
                registrationRequestDao,
                cancellationToken
            );

            await UploadRegistrationRequestDocumentsByType(
                [.. command.RegistrationRequestCniAttachments],
                RegistrationRequestDocumentType.IdentityDocument,
                registrationRequestDao,
                cancellationToken
            );

            await UploadRegistrationRequestDocumentsByType(
                [.. command.Photo],
                RegistrationRequestDocumentType.Photo,
                registrationRequestDao,
                cancellationToken
            );
        }

        private async Task UploadRegistrationRequestDocumentsByType(
            List<IFormFile> attachements,
            RegistrationRequestDocumentType documentType,
            RegistrationRequestDao existingRegistrationRequest,
            CancellationToken cancellationToken
        )
        {
            var existingDocumentsForType =
                existingRegistrationRequest
                    .RegistrationRequestDocuments?.Where(doc =>
                        doc.RegistrationRequestDocumentType == documentType
                    )
                    .ToList()
                ?? [];

            var newDocuments = new List<RegistrationRequestDocumentDao>();

            foreach (var file in attachements)
            {
                var existingDocument = existingDocumentsForType.FirstOrDefault(doc =>
                    string.Equals(doc.Document.FileName, file.FileName)
                );

                Guid? existingDocumentId = existingDocument?.DocumentId;

                using var stream = file.OpenReadStream();

                var documentId = await fileService.UploadFileNoTransactionAsync(
                    stream,
                    file.FileName,
                    file.ContentType,
                    context,
                    cancellationToken,
                    existingDocumentId
                );

                if (existingDocument is null)
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
            if (existingRegistrationRequest == null)
                return;

            var obsoleteDocuments = existingRegistrationRequest
                .RegistrationRequestDocuments.Where(doc =>
                    doc.RegistrationRequestDocumentType == documentType && !newAttachmentsIds.Contains(doc.Id)
                )
                .ToList();

            foreach (var document in obsoleteDocuments)
            {
                await fileService.DeleteFileAsync(document.DocumentId, context, cancellationToken);
            }
        }
    }
}
