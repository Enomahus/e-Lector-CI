using System.ComponentModel.DataAnnotations.Schema;
using Infrastructure.Persistence.Common;

namespace Infrastructure.Persistence.Entities;

public class PollingStationDao : EntityBaseDao<long>
{
    public int StationNumber { get; set; } // Bureau No: 04
    public string Name { get; set; } // Lieu de vote: LYON

    // Relation vers la localisation
    public long GeographicAreaId { get; set; }

    [ForeignKey(nameof(GeographicAreaId))]
    public GeographicAreaDao Location { get; set; }

    public virtual ICollection<ElectorDao> Electors { get; set; }
}
