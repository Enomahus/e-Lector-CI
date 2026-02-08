using Application.Common.Enums;
using Infrastructure.Persistence.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Infrastructure.Persistence.Entities;

public class GeographicAreaDao : EntityBaseDao<long>
{
    [Required, MaxLength(100)]
    public string Name { get; set; }

    [Required]
    public LocationLevel Level { get; set; }

    // Auto-référence : le parent de cette zone
    public long? ParentId { get; set; }

    [ForeignKey(nameof(ParentId))]
    public virtual GeographicAreaDao Parent { get; set; }

    // Liste des zones enfants (ex: la France a plusieurs villes)
    public virtual ICollection<GeographicAreaDao> Children { get; set; } = [];

    // Liste des bureaux de vote rattachés à cette zone précise (souvent le dernier niveau)
    public virtual ICollection<PollingStationDao> PollingStations { get; set; } = [];
}
