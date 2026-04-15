using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Application.Features.RegistrationRequests.Common
{
    public class RegistrationRequestCommandBase
    {
        public RegistrationRequestModel? RegistrationRequest {  get; set; }
        [JsonIgnore]
        public ICollection<IFormFile> RegistrationRequestCertificateAttachments { get; set; } = [];
        [JsonIgnore]
        public ICollection<IFormFile> RegistrationRequestCniAttachments { get; set; } = [];
        [JsonIgnore]
        public ICollection<IFormFile> Photo {  get; set; } = [];
         
    }
}
