using Application.Common.Enums;
using Application.Features.Common.Citizen;
using Infrastructure.Persistence.Entities;
using Infrastructure.Persistence.SQLServer.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.RegistrationRequests.Common
{
    public abstract class BaseRegistrationRequestsHandler<TQuery>(
        ReadOnlyDbContext context,
        TimeProvider timeProvider
    )
        where TQuery : IRegistrationRequestQuery
    {
        protected readonly ReadOnlyDbContext _context = context;
        protected readonly TimeProvider _timeProvider = timeProvider;

        protected async Task<(List<GetRegistrationRequestsResponseModel> Items, int TotalCount)> GetDataAsync(
            IQueryable<RegistrationRequestDao> query,
            int pageIndex,
            int pageSize,
            CancellationToken cancellationToken,
            Guid? userId = null
        )
        {
            if (userId.HasValue)
            {
                query = query.Where(x => x.AuthorId == userId);
            }

            // Count exécuté côté SQL avant projection pour des performances optimales.
            var totalCount = await query.CountAsync(cancellationToken);

            if (totalCount == 0 || pageIndex * pageSize >= totalCount)
            {
                return ([], totalCount);
            }

            // Pagination côté SQL avant matérialisation pour éviter de charger toutes les lignes.
            var rawData = await query
                .Skip(pageIndex * pageSize)
                .Take(pageSize)
                .Select(x => new
                {
                    x.Id,
                    x.Reference,
                    x.SubmissionDate,
                    x.Status,
                    x.ConstituencyId,
                    ConstituencyName = x.Constituency.Wording,
                    Comment = x.ReasonForRejection,
                    x.Citizen, // On récupère l'entité Citizen brute pour le mapping C# ensuite
                    x.AuthorId,
                    x.Author,
                    // On filtre les documents en une seule fois pour éviter les FirstOrDefault multiples
                    Documents = x
                        .RegistrationRequestDocuments.Where(d =>
                            d.RegistrationRequestDocumentType
                                == RegistrationRequestDocumentType.CertificateOfNationality
                            || d.RegistrationRequestDocumentType
                                == RegistrationRequestDocumentType.IdentityDocument
                            || d.RegistrationRequestDocumentType == RegistrationRequestDocumentType.Photo
                        )
                        .Select(d => new { d.RegistrationRequestDocumentType, d.DocumentId })
                        .ToList(),
                })
                .ToListAsync(cancellationToken);

            var items = rawData
                .Select(x => new GetRegistrationRequestsResponseModel
                {
                    Id = x.Id,
                    Reference = x.Reference,
                    SubmissionDate = x.SubmissionDate,
                    Status = x.Status,
                    ConstituencyId = x.ConstituencyId,
                    ConstituencyName = x.ConstituencyName,
                    Comment = x.Comment,
                    AuthorName = $"{x.Author?.FirstName} {x.Author?.LastName}",
                    Citizen = CitizenModel.FromDao(x.Citizen),
                    CanBeDeleted =
                        userId.HasValue
                        && x.AuthorId == userId.Value
                        && x.Status == RegistrationStatus.ToBeProcessed,
                    CertificateOfNationalityDocumentId = x
                        .Documents.FirstOrDefault(d =>
                            d.RegistrationRequestDocumentType
                            == RegistrationRequestDocumentType.CertificateOfNationality
                        )
                        ?.DocumentId.ToString(),
                    CertificateOfNationalityDocumentName = null,
                    IdentityDocumentId = x
                        .Documents.FirstOrDefault(d =>
                            d.RegistrationRequestDocumentType
                            == RegistrationRequestDocumentType.IdentityDocument
                        )
                        ?.DocumentId.ToString(),
                    IdentityDocumentName = null,
                    PhotoId = x
                        .Documents.FirstOrDefault(d =>
                            d.RegistrationRequestDocumentType == RegistrationRequestDocumentType.Photo
                        )
                        ?.DocumentId.ToString(),
                    PhotoName = null,
                })
                .ToList();

            return (items, totalCount);
        }

        protected async Task AddFileNameAsync(
            IEnumerable<GetRegistrationRequestsResponseModel> data,
            CancellationToken cancellationToken
        )
        {
            //Certificat de nationalité
            var nationalCertificateIds = data.Where(x =>
                    !string.IsNullOrEmpty(x.CertificateOfNationalityDocumentId)
                )
                .Select(x => Guid.Parse(x.CertificateOfNationalityDocumentId!))
                .Distinct()
                .ToList();
            if (nationalCertificateIds.Count > 0)
            {
                var certificateFileNames = await _context
                    .Documents.Where(d => nationalCertificateIds.Contains(d.Id))
                    .Select(d => new { d.Id, d.FileName })
                    .ToDictionaryAsync(d => d.Id.ToString(), d => d.FileName, cancellationToken);

                foreach (
                    var item in data.Where(x => !string.IsNullOrEmpty(x.CertificateOfNationalityDocumentId))
                )
                {
                    if (
                        certificateFileNames.TryGetValue(
                            item.CertificateOfNationalityDocumentId!,
                            out var fileName
                        )
                    )
                    {
                        item.CertificateOfNationalityDocumentName = fileName;
                    }
                }
            }

            //CNI
            var identityDocumentsIds = data.Where(x => !string.IsNullOrEmpty(x.IdentityDocumentId))
                .Select(x => Guid.Parse(x.IdentityDocumentId!))
                .Distinct()
                .ToList();
            if (identityDocumentsIds.Count > 0)
            {
                var identityFileNames = await _context
                    .Documents.Where(d => identityDocumentsIds.Contains(d.Id))
                    .Select(d => new { d.Id, d.FileName })
                    .ToDictionaryAsync(d => d.Id.ToString(), d => d.FileName, cancellationToken);

                foreach (var item in data.Where(x => !string.IsNullOrEmpty(x.IdentityDocumentId)))
                {
                    if (identityFileNames.TryGetValue(item.IdentityDocumentId!, out var fileName))
                    {
                        item.IdentityDocumentName = fileName;
                    }
                }
            }

            //Photo
            var photoIds = data.Where(x => !string.IsNullOrEmpty(x.PhotoId))
                .Select(x => Guid.Parse(x.PhotoId!))
                .Distinct()
                .ToList();
            if (photoIds.Count > 0)
            {
                var photoFileNames = await _context
                    .Documents.Where(d => photoIds.Contains(d.Id))
                    .Select(d => new { d.Id, d.FileName })
                    .ToDictionaryAsync(d => d.Id.ToString(), d => d.FileName, cancellationToken);

                foreach (var item in data.Where(x => !string.IsNullOrEmpty(x.PhotoId)))
                {
                    if (photoFileNames.TryGetValue(item.PhotoId!, out var fileName))
                    {
                        item.PhotoName = fileName;
                    }
                }
            }
        }
    }

    public interface IRegistrationRequestQuery { }
}
