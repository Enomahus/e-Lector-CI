using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Application.Common.Enums;
using Infrastructure.Persistence.Common;

namespace Infrastructure.Persistence.Entities
{
    public class CitizenDao : EntityBaseDao<Guid>
    {
        public MaritalStatus? MaritalStatus { get; set; }

        public string MarriedName { get; set; }

        [Required, MaxLength(100)]
        public string LastName { get; set; }

        [Required, MaxLength(150)]
        public string FirstName { get; set; }

        public Gender Gender { get; set; }

        [Required]
        public string Nationality { get; set; } = "Ivoirienne";

        [Required]
        public DateOnly BirthDate { get; set; }

        [Required, MaxLength(100)]
        public string BirthPlace { get; set; }

        public string Profession { get; set; }

        // Adresses
        public string PhysicalAddress { get; set; }
        public string PostalAddress { get; set; }

        [InverseProperty(nameof(FiliationDao.Parent))]
        public virtual ICollection<FiliationDao> FiliationAsParent { get; set; } = [];

        [InverseProperty(nameof(FiliationDao.Citizen))]
        public virtual ICollection<FiliationDao> FiliationsAsSubject { get; set; } = [];

        // Relation un-à-un vers l'électeur
        public ElectorDao Elector { get; set; }
    }
}
