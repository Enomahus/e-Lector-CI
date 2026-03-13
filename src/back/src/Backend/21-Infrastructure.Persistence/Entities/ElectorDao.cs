using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Application.Common.Enums;
using Infrastructure.Persistence.Common;

namespace Infrastructure.Persistence.Entities
{
    public class ElectorDao : EntityBaseDao<Guid>
    {
        [MaxLength(50)]
        public string VoterNumber { get; set; } // V 0034 6601 11

        public DateTimeOffset RegistrationDate { get; set; }

        public Guid CitizenId { get; set; }
        public CitizenDao Citizen { get; set; }

        public long ConstituencyId { get; set; }

        [ForeignKey(nameof(ConstituencyId))]
        public ConstituencyDao Constituencies { get; set; }

        // Relations
        //public long? PollingStationId { get; set; }

        //[ForeignKey(nameof(PollingStationId))]
        //public PollingStationDao PollingStation { get; set; }
    }
}
