using Application.Features.RegistrationRequests.Common;

namespace Application.Features.RegistrationRequests.GetRegistrationRequestsForAdmin
{
    public class GetRegistrationRequestsForAdminResponse : GetRegistrationRequestsResponseModel
    {
        public required string AuthorName { get; set; }
    }
}
