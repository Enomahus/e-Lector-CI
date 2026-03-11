using Application.Common.Enums;
using Infrastructure.Persistence.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Infrastructure.Persistence.Entities
{
    public class ElectorDao : EntityBaseDao<Guid>
    {
        [MaxLength(50)]
        public string VoterNumber { get; set; } // V 0034 6601 11

        [Required, MaxLength(100)]
        public string LastName { get; set; }

        public string MarriedName { get; set; }

        [Required, MaxLength(150)]
        public string FirstNames { get; set; }

        public Gender Gender { get; set; }

        [Required,DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }

        [Required, MaxLength(100)]
        public string PlaceOfBirth { get; set; }

        public string Profession { get; set; }

        // Adresses
        public string PhysicalAddress { get; set; }
        public string PostalAddress { get; set; }

        // Relations
        public long? PollingStationId { get; set; }
        [ForeignKey(nameof(PollingStationId))]
        public PollingStationDao PollingStation { get; set; }
    }
}
