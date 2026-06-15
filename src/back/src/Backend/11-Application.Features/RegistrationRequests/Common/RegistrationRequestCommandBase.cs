using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Application.Features.RegistrationRequests.Common
{
    public class RegistrationRequestCommandBase
    {
        public RegistrationRequestModel? RegistrationRequest { get; set; }

        [JsonIgnore]
        public IFormFile? RegistrationRequestCniOrCertificateAttachments { get; set; }

        [JsonIgnore]
        public IFormFile? Photo { get; set; }
    }
}
