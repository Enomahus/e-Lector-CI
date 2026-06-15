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

        public static GetRegistrationRequestResponse FromDao(
            RegistrationRequestDao dao,
            UserDao userDao,
            DateTimeOffset dateNow
        )
        {
            return new GetRegistrationRequestResponse()
            {
                Id = dao.Id,
                Reference = dao.Reference,
                Status = dao.Status,
                RegistrationRequestType = dao.RequestType,
                SoumissionDate = dao.SubmissionDate,
                ConstituencyId = dao.ConstituencyId,
                ReasonForRejection = dao.ReasonForRejection,
                Citizen = CitizenModel.FromDao(dao.Citizen),
                Author = UserModel.FromDao(userDao, dateNow),

                IdentityDocumentOrCertificateIds = dao
                    .RegistrationRequestDocuments.FirstOrDefault(d =>
                        d.RegistrationRequestDocumentType
                        == RegistrationRequestDocumentType.IdentityDocumentOrNationalCertificate
                    )
                    ?.DocumentId,
                PhotoIds = dao
                    .RegistrationRequestDocuments.FirstOrDefault(d =>
                        d.RegistrationRequestDocumentType == RegistrationRequestDocumentType.Photo
                    )
                    ?.DocumentId,
            };
        }
    }
}
