using Application.Common.Enums;
using Infrastructure.Persistence.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Infrastructure.Persistence.Entities;

public class RegistrationRequestDocumentDao : EntityBaseDao<Guid>
{
    [Required]
    public DocumentType RegistrationRequestDocumentType { get; set; }
    public Guid RegistrationRequestId { get; set; }

    [ForeignKey(nameof(RegistrationRequestId))]
    public RegistrationRequestDao RegistrationRequest { get; set; }

    public Guid DocumentId { get; set; }

    [ForeignKey(nameof(DocumentId))]
    public DocumentDao Document { get; set; }
}
