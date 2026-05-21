using Application.Common.Enums;
using Application.Features.RegistrationRequests.Common;
using Pcea.Core.Net.Authorization.Application.Attributes;

namespace Application.Features.RegistrationRequests.GetRegistrationRequestsForManagement
{
    [WithPermission([nameof(AppPermission.GetRegistrationRequestsForManagement)])]
    public class GetRegistrationRequestsForManagementResponse : GetRegistrationRequestsResponseModel
    {
        public required string AuthorName { get; set; }
    }
}
