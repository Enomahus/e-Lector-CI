using Application.Common.Enums;
using Application.Features.Common.Citizen;
using Application.Features.Users.Common;
using Infrastructure.Persistence.Entities;

namespace Application.Features.RegistrationRequests.Common
{
    public class RegistrationRequestModel
    {
        public Guid? Id { get; set; }
        //public DateTimeOffset SoumissionDate { get; set; }
        //public RegistrationStatus Status { get; set; }
        public string? ReasonForRejection { get; set; }
        public long? ConstituencyId { get; set; }
        public UserModel? Author { get; set; }
        public CitizenModel? Citizen { get; set; }

        public ICollection<Guid>? CertificateOfNationalityDocumentIds { get; set; }
        public ICollection<Guid>? IdentityDocumentIds { get; set; }


        public static RegistrationRequestModel FromDao(RegistrationRequestDao dao, TimeProvider timeProvider)  
        {                     

            var dateNow = timeProvider.GetUtcNow();

            return new RegistrationRequestModel()
            {
                Id = dao.Id,
                //Status = dao.Status,
                //SoumissionDate = dao.SoumissionDate,
                ReasonForRejection = dao.ReasonForRejection,
                Author = UserModel.FromDao(dao.Author, dateNow),
                Citizen = CitizenModel.FromDao(dao.Citizen),
                ConstituencyId = dao.ConstituencyId
            };                                              
        }

        public RegistrationRequestDao ToDao(long constituencyId)
        {
            return new RegistrationRequestDao()
            {
                Id = Id ?? Guid.NewGuid(),
                //SoumissionDate = SoumissionDate,
                //Status = Status,
                ReasonForRejection = ReasonForRejection,
                ConstituencyId = constituencyId,
                Citizen = Citizen?.ToDao(),
            };
        }

    }
}
