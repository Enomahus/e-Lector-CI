using Application.Common.Enums;
using Application.Features.Common.Citizen;

namespace Application.Features.RegistrationRequests.Common
{
    public class GetRegistrationRequestsResponseModel
    {
        public required Guid Id { get; set; }
        public required string Reference { get; set; }
        public required DateTimeOffset SubmissionDate { get; set; }
        public required RegistrationStatus Status { get; set; }
        public required long ConstituencyId { get; set; }
        public required string ConstituencyName { get; set; }
        public required string Comment { get; set; }
        public required CitizenModel Citizen { get; set; }
        public required bool CanBeDeleted { get; set; }
    }
}
