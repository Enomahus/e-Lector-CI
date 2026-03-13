using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Application.Common.Enums;
using Infrastructure.Persistence.Common;

namespace Infrastructure.Persistence.Entities;

public class ConstituencyDao : EntityBaseDao<long>
{
    [Required]
    public string Code { get; set; }

    [Required, MaxLength(50)]
    public string Name { get; set; }

    [Required]
    public LocationLevel Level { get; set; }

    // Auto-référence : le parent de cette zone
    public long? ParentId { get; set; }

    [ForeignKey(nameof(ParentId))]
    public virtual ConstituencyDao Parent { get; set; }

    // Liste des zones enfants (ex: la France a plusieurs villes)
    public virtual ICollection<ConstituencyDao> Children { get; set; } = [];

    // Liste des bureaux de vote rattachés à cette zone précise (souvent le dernier niveau)
    // public virtual ICollection<PollingStationDao> PollingStations { get; set; } = [];

    public virtual ICollection<ElectorDao> Electors { get; set; } = [];
}
