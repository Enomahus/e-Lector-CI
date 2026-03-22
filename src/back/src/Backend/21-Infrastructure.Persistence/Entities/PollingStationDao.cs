using System.ComponentModel.DataAnnotations.Schema;
using Infrastructure.Persistence.Common;

namespace Infrastructure.Persistence.Entities;

public class PollingStationDao : EntityBaseDao<long>, ITimestampedEntity
{
    public required string StationNumber { get; set; } // Bureau No: 04
    public required string Wording { get; set; } // Lieu de vote: LYON

    // Relation vers la circonscription (ex: Lieu de vote)
    public long ConstituencyId { get; set; }

    [ForeignKey(nameof(ConstituencyId))]
    public virtual ConstituencyDao Constituency { get; set; } = null!;

    public virtual ICollection<ElectorDao> Electors { get; set; } = [];
    public DateTimeOffset ModifiedAt { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}
