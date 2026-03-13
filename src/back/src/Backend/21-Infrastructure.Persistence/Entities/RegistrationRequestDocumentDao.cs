using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Application.Common.Enums;
using Infrastructure.Persistence.Common;

namespace Infrastructure.Persistence.Entities;

public class RegistrationRequestDocumentDao : EntityBaseDao<Guid>
{
    [StringLength(60)]
    public string DocumentNumber { get; set; }
    public DateOnly? IssueDate { get; set; }
    public DateOnly? ExpiryDate { get; set; }

    [Required]
    public DocumentType RegistrationRequestDocumentType { get; set; }
    public Guid RegistrationRequestId { get; set; }

    [ForeignKey(nameof(RegistrationRequestId))]
    public RegistrationRequestDao RegistrationRequest { get; set; }

    public Guid DocumentId { get; set; }

    [ForeignKey(nameof(DocumentId))]
    public DocumentDao Document { get; set; }
}
