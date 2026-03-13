using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Application.Common.Enums;
using Infrastructure.Persistence.Common;

namespace Infrastructure.Persistence.Entities;

public class FiliationDao : EntityBaseDao<Guid>
{
    // Le citoyen Sujet de la filiation (celui dont on décrit le parent)
    public Guid CitizenId { get; set; }

    [ForeignKey(nameof(CitizenId))]
    public CitizenDao Citizen { get; set; }

    // Le parent (père/mère) référencé comme un autre citoyen
    public Guid ParentId { get; set; }

    [ForeignKey(nameof(ParentId))]
    public virtual CitizenDao Parent { get; set; }

    [Required]
    public FiliationType FiliationType { get; set; }
}
