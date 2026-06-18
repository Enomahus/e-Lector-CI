using Application.Common.Enums;
using Infrastructure.Persistence.Entities;

namespace Application.Features.Common.RegistrationRequestDocument
{
    public class RegistrationRequestDocumentModel
    {
        public Guid Id { get; set; }
        public Guid? DocumentId { get; set; }
        public Guid? RegistrationRequestId { get; set; }
        public RegistrationRequestDocumentType DocumentType { get; set; }
        public DateTimeOffset? IssueDate { get; set; }
        public DateTimeOffset? ExpiryDate { get; set; }
        public string? PartNumber { get; set; }
        public string? IssuePlace { get; set; }

        public static RegistrationRequestDocumentModel FromDao(RegistrationRequestDocumentDao dao)
        {
            return new RegistrationRequestDocumentModel
            {
                Id = dao.Id,
                DocumentId = dao.DocumentId,
                DocumentType = dao.RegistrationRequestDocumentType,
                RegistrationRequestId = dao.RegistrationRequestId,
                PartNumber = dao.PartNumber,
                IssueDate = dao.IssueDate,
                ExpiryDate = dao.ExpiryDate,
                IssuePlace = dao.IssuePlace,
            };
        }

        public RegistrationRequestDocumentDao ToDao()
        {
            return new RegistrationRequestDocumentDao
            {
                Id = Id,
                DocumentId = DocumentId ?? Guid.NewGuid(),
                RegistrationRequestId = RegistrationRequestId ?? Guid.NewGuid(),
                RegistrationRequestDocumentType = DocumentType,
                PartNumber = PartNumber,
                IssueDate = IssueDate,
                ExpiryDate = ExpiryDate,
                IssuePlace = IssuePlace,
            };
        }
    }
}
