using Infrastructure.Persistence.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.Metrics;
using System.Text;

namespace Infrastructure.Persistence.Entities
{
    public class PollingStationDao: EntityBaseDao<long>
    {
        public int StationNumber { get; set; } // Bureau No: 04
        public string Name { get; set; } // Lieu de vote: LYON

        // Relation vers la localisation
        public long GeographicAreaId { get; set; }
        [ForeignKey(nameof(GeographicAreaId))]
        public GeographicAreaDao Location { get; set; }

        public ICollection<ElectorDao> Electors { get; set; }
    }
}
