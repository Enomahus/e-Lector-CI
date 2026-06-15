using Application.Common.Enums;
using Application.Exceptions;
using Application.Exceptions.Auth;
using Application.Features.Common.RegistrationRequestDocument;
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
                    .RegistrationRequests.AsSplitQuery()
                    .Include(r => r.RegistrationRequestDocuments)
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
                registrationRequest.Status == RegistrationStatus.Approved
                || registrationRequest.Status == RegistrationStatus.Rejected
            )
            {
                throw new Exception("This request has already been processed and cannot be changed");
            }

            if (!userIsSuperAdmin && registrationRequest.AuthorId != currentUser.Id)
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
            var cniOrCetificateDocument = command.RegistrationRequest?.Documents?.FirstOrDefault(d =>
                d.DocumentType == RegistrationRequestDocumentType.IdentityDocumentOrNationalCertificate
            );

            //Remove old document
            await RemoveObsoleteDocuments(
                command.RegistrationRequest?.IdentityDocumentOrCertificateIds,
                RegistrationRequestDocumentType.IdentityDocumentOrNationalCertificate,
                registrationRequestDao,
                cancellationToken
            );
            await RemoveObsoleteDocuments(
                command.RegistrationRequest?.PhotoIds,
                RegistrationRequestDocumentType.Photo,
                registrationRequestDao,
                cancellationToken
            );

            //Upload documents
            await UploadRegistrationRequestDocumentsByType(
                command.RegistrationRequestCniOrCertificateAttachments,
                RegistrationRequestDocumentType.IdentityDocumentOrNationalCertificate,
                registrationRequestDao,
                cancellationToken,
                cniOrCetificateDocument
            );

            await UploadRegistrationRequestDocumentsByType(
                command.Photo,
                RegistrationRequestDocumentType.Photo,
                registrationRequestDao,
                cancellationToken
            );
        }

        public async Task UploadRegistrationRequestDocumentsByType(
            IFormFile? attachement,
            RegistrationRequestDocumentType documentType,
            RegistrationRequestDao existingRegistrationRequest,
            CancellationToken cancellationToken,
            RegistrationRequestDocumentModel? cniOrCetificateDocument = null
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

            //foreach (var file in attachements)
            //{
            if (attachement is { Length: > 0 })
            {
                var existingDocument = existingDocumentsForType.FirstOrDefault(doc =>
                    string.Equals(doc.Document.FileName, attachement.FileName)
                );

                Guid? existingDocumentId = existingDocument?.DocumentId;

                using var stream = attachement.OpenReadStream();

                var documentId = await fileService.UploadFileNoTransactionAsync(
                    stream,
                    attachement.FileName,
                    attachement.ContentType,
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
                    if (documentType == RegistrationRequestDocumentType.IdentityDocumentOrNationalCertificate)
                    {
                        newDocument.PartNumber = cniOrCetificateDocument?.PartNumber;
                        newDocument.IssueDate = cniOrCetificateDocument?.IssueDate;
                        newDocument.ExpiryDate = cniOrCetificateDocument?.ExpiryDate;
                        newDocument.IssuePlace = cniOrCetificateDocument?.IssuePlace;
                    }

                    newDocuments.Add(newDocument);
                }
            }
            //}

            context.RegistrationRequestDocuments.AddRange(newDocuments);
            await context.SaveChangesAsync(cancellationToken);
        }

        protected async Task RemoveObsoleteDocuments(
            Guid? newAttachmentsIds,
            RegistrationRequestDocumentType documentType,
            RegistrationRequestDao? existingRegistrationRequest,
            CancellationToken cancellationToken = default
        )
        {
            if (existingRegistrationRequest == null || newAttachmentsIds is null)
                return;

            var obsoleteDocuments = existingRegistrationRequest
                .RegistrationRequestDocuments.Where(doc =>
                    doc.RegistrationRequestDocumentType == documentType && doc.Id == newAttachmentsIds
                )
                .ToList();

            foreach (var document in obsoleteDocuments)
            {
                await fileService.DeleteFileAsync(document.DocumentId, context, cancellationToken);
            }
        }
    }
}
