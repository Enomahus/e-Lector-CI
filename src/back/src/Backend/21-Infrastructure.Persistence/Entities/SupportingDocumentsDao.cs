using Application.Common.Enums;
using Infrastructure.Persistence.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Infrastructure.Persistence.Entities;

public class SupportingDocumentsDao : EntityBaseDao<Guid>
{
    [StringLength(60)]
    public string DocumentNumber { get; set; }
    public string UrlStockage { get; set; }
    public DateTimeOffset DateUpload { get; set; }
    public bool IsValid { get; set; }

    [Required]
    public DocumentType SupportingDocumentType { get; set; }
    public Guid RegistrationRequestId { get; set; }

    [ForeignKey(nameof(RegistrationRequestId))]
    public RegistrationRequestDao RegistrationRequest { get; set; }
}
