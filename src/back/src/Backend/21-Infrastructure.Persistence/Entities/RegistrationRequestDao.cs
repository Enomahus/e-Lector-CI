using Application.Common.Enums;
using Infrastructure.Persistence.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Infrastructure.Persistence.Entities;

public class RegistrationRequestDao : EntityBaseDao<Guid>
{
    [MaxLength(25)]
    [Required]
    public string Reference { get; set; }
    public DateTimeOffset SoumissionDate { get; set; }
    public RegistrationStatus Status { get; set; }
    public string ReasonForRejection { get; set; }
    public Guid? AuthorId { get; set; }

    [ForeignKey(nameof(AuthorId))]
    public UserDao Author { get; set; }
    public Guid? LastUpdaterId { get; set; }

    [ForeignKey(nameof(LastUpdaterId))]
    public UserDao LastUpdater { get; set; }
    public Guid? ElectorId { get; set; }

    [ForeignKey(nameof(ElectorId))]
    public ElectorDao Elector { get; set; }
    public virtual ICollection<SupportingDocumentsDao> SupportingDocuments { get; set; } = [];
}
