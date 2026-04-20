using Application.Features.RegistrationRequests.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NJsonSchema.Annotations;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace Application.Features.RegistrationRequests.UpdateRegistrationRequest
{
    [ExcludeFromCodeCoverage]
    public class UpdateRegistrationRequestFromData
    {
        [FromRoute]
        public Guid? Id { get; set; }
        [FromForm]
        [JsonSchemaType(typeof(RegistrationRequestModel))]
        public string? RegistrationRequestJson { get; set; }
        [FromForm]
        public ICollection<IFormFile> RegistrationRequestCertificateAttachments { get; set; } = [];
        [FromForm]
        public ICollection<IFormFile> RegistrationRequestCniAttachments { get; set; } = [];
        public ICollection<IFormFile> Photo { get; set; } = [];

        public RegistrationRequestModel? GetRegistrationRequest()
        {
            if(string.IsNullOrWhiteSpace(RegistrationRequestJson)) return null;

            return JsonSerializer.Deserialize<RegistrationRequestModel>(
                RegistrationRequestJson,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
    }
}
