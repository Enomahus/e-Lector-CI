using Application.Common.Enums;
using Infrastructure.Persistence.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Infrastructure.Persistence.Entities
{
    public class CitizenDao : EntityBaseDao<Guid>, ITimestampedEntity
    {
        
        [Required, MaxLength(100)]
        public string LastName { get; set; }

        [Required, MaxLength(150)]
        public string FirstName { get; set; }
        public Gender Gender { get; set; }
        public string Nationality { get; set; } 
        [Required]
        public DateOnly BirthDate { get; set; }

        [Required, MaxLength(100)]
        public string BirthPlace { get; set; }
        public MaritalStatus MaritalStatus { get; set; }
        public string MarriedName { get; set; }

        public string Profession { get; set; }
        // Adresses
        public string PhysicalAddress { get; set; }
        public string PostalAddress { get; set; }

        public DateTimeOffset ModifiedAt { get; set; }
        public DateTimeOffset CreatedAt { get; set; }

        //Filiation
        public Guid? FatherId { get; set; }
        [ForeignKey(nameof(FatherId))]
        public virtual CitizenDao Father {  get; set; }
        public Guid? MotherId { get; set; }
        [ForeignKey(nameof(MotherId))]
        public virtual CitizenDao Mother { get; set; }


        // Relation un-à-un vers l'électeur
        public virtual ElectorDao ElectorProfil { get; set; }
        public virtual ICollection<RegistrationRequestDao> RegistrationRequests { get; set; }
    }
}
