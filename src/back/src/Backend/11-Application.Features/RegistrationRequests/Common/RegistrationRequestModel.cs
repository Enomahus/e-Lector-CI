using Application.Common.Enums;
using Application.Features.Common.Citizen;
using Application.Features.Users.Common;
using Infrastructure.Persistence.Entities;

namespace Application.Features.RegistrationRequests.Common
{
    public class RegistrationRequestModel
    {
        public Guid? Id { get; set; }
        public string? ReasonForRejection { get; set; }
        public long? ConstituencyId { get; set; }
        public UserModel? Author { get; set; }
        public CitizenModel? Citizen { get; set; }
        public RegistrationRequestType RegistrationRequestType { get; set; }

        public Guid? CertificateOfNationalityDocumentIds { get; set; }
        public Guid? IdentityDocumentIds { get; set; }
        public Guid? PhotoIds { get; set; }

        public static RegistrationRequestModel FromDao(RegistrationRequestDao dao, TimeProvider timeProvider)
        {
            var dateNow = timeProvider.GetUtcNow();

            return new RegistrationRequestModel()
            {
                Id = dao.Id,
                ReasonForRejection = dao.ReasonForRejection,
                RegistrationRequestType = dao.RequestType,
                Author = UserModel.FromDao(dao.Author, dateNow),
                Citizen = CitizenModel.FromDao(dao.Citizen),
                ConstituencyId = dao.ConstituencyId,
            };
        }

        public RegistrationRequestDao ToDao(long constituencyId)
        {
            return new RegistrationRequestDao()
            {
                Id = Id ?? Guid.NewGuid(),
                ReasonForRejection = ReasonForRejection,
                ConstituencyId = constituencyId,
                Citizen = Citizen?.ToDao(),
                RequestType = RegistrationRequestType,
            };
        }
    }
}
