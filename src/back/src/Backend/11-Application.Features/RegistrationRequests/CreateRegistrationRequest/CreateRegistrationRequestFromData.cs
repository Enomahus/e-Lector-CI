using Application.Features.RegistrationRequests.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NJsonSchema.Annotations;
using System.Diagnostics.CodeAnalysis;

namespace Application.Features.RegistrationRequests.CreateRegistrationRequest;

[ExcludeFromCodeCoverage]
public class CreateRegistrationRequestFromData
{
    [FromForm]
    [JsonSchemaType(typeof(RegistrationRequestModel))]
    public string? RegistrationRequestJson { get; set; }
    [FromForm]
    public ICollection<IFormFile> RegistrationRequestCertificateAttachments { get; set; } = [];
    [FromForm]
    public ICollection<IFormFile> RegistrationRequestCniAttachments { get; set; } = [];
}
