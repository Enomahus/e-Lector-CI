using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;
using Application.Features.RegistrationRequests.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NJsonSchema.Annotations;

namespace Application.Features.RegistrationRequests.CreateRegistrationRequest;

[ExcludeFromCodeCoverage]
public class CreateRegistrationRequestFromData
{
    [FromForm]
    [JsonSchemaType(typeof(RegistrationRequestModel))]
    public string? RegistrationRequestJson { get; set; }

    [FromForm]
    public IFormFile? RegistrationRequestCertificateAttachments { get; set; }

    [FromForm]
    public IFormFile? RegistrationRequestCniAttachments { get; set; }

    [FromForm]
    public IFormFile? Photo { get; set; }

    public RegistrationRequestModel? GetRegistrationRequest()
    {
        if (string.IsNullOrWhiteSpace(RegistrationRequestJson))
            return null;

        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        options.Converters.Add(new JsonStringEnumConverter());

        return JsonSerializer.Deserialize<RegistrationRequestModel>(RegistrationRequestJson, options);
    }
}
