using Application.Common.Enums;
using Application.Features.Common.Citizen;
using Application.Features.RegistrationRequests.Common;
using Application.Features.Users.Common;
using Infrastructure.Persistence.Entities;

namespace Application.Features.RegistrationRequests.GetRegistrationRequest
{
    public class GetRegistrationRequestResponse : RegistrationRequestModel
    {
        public string? Reference { get; set; }
        public DateTimeOffset SoumissionDate { get; set; }
        public RegistrationStatus Status { get; set; }

        public static GetRegistrationRequestResponse FromDao(RegistrationRequestDao dao, UserDao userDao, DateTimeOffset dateNow)
        {
            return new GetRegistrationRequestResponse()
            {

                Reference = dao.Reference,
                Status = dao.Status,
                SoumissionDate = dao.SoumissionDate,
                ConstituencyId = dao.ConstituencyId,
                ReasonForRejection = dao.ReasonForRejection,
                Citizen = CitizenModel.FromDao(dao.Citizen),
                Author = UserModel.FromDao(userDao, dateNow),
                CertificateOfNationalityDocumentIds = dao.RegistrationRequestDocuments
                .Where(d => d.RegistrationRequestDocumentType == RegistrationRequestDocumentType.CertificateOfNationality)
                .Select(d => d.DocumentId)
                .ToList(),
                IdentityDocumentIds = dao.RegistrationRequestDocuments
                    .Where(d => d.RegistrationRequestDocumentType == RegistrationRequestDocumentType.IdentityDocument)
                    .Select(d => d.DocumentId)
                    .ToList(),
                PhotoIds = dao.RegistrationRequestDocuments
                    .Where(d => d.RegistrationRequestDocumentType == RegistrationRequestDocumentType.Photo)
                    .Select(d => d.DocumentId)
                    .ToList()
            };            
        }
    }
}
