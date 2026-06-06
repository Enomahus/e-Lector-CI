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

        public static RegistrationRequestDocumentModel From(RegistrationRequestDocumentDao dao)
        {
            return new RegistrationRequestDocumentModel
            {
                Id = dao.Id,
                DocumentId = dao.DocumentId,
                DocumentType = dao.RegistrationRequestDocumentType,
                RegistrationRequestId = dao.RegistrationRequestId,
            };
        }
    }
}
