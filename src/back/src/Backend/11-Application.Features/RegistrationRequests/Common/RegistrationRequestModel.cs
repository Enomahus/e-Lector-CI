using Application.Common.Enums;
using Application.Features.Common.Elector;
using Application.Features.Users.Common;

namespace Application.Features.RegistrationRequests.Common
{
    public class RegistrationRequestModel
    {
        public long? Id { get; set; }
        public DateTimeOffset SoumissionDate { get; set; }
        public RegistrationStatus Status { get; set; }
        public string? ReasonForRejection { get; set; }
        public UserModel? Author { get; set; }
        public ElectorModel? Elector { get; set; }
    }
}
