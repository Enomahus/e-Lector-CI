using Application.Common.Enums;
using Application.Models;
using Infrastructure.Persistence.Entities;
using MediatR;

namespace Application.Features.Common.Elector
{
    public class ElectorModel 
    {       
        public DateTimeOffset? RegistrationDate { get; set; }
        public ElectorStatus Status { get; set; }
        public Guid CitizenId { get; set; }
        public long PollingStationId { get; set; }
        
        public ElectorDao ToDao(long? pollingStationId, Guid? citizenId)
        {
            return new ElectorDao
            {                
                RegistrationDate = RegistrationDate!.Value,
                Status = Status,
                PollingStationId = pollingStationId!.Value,
                Id = citizenId!.Value
            };
        }

        public ElectorDao ToDao(PollingStationDao? pollingStation, CitizenDao? citizen)
        {
            var elector = ToDao(pollingStation?.Id, citizen?.Id);
            elector.PollingStation = pollingStation;
            elector.Citizen = citizen;
            return elector;
        }

        public static ElectorModel From(ElectorDao elector)
        {
            return new ElectorModel
            {
                RegistrationDate = elector.RegistrationDate,
                Status = elector.Status,
                CitizenId = elector.Citizen.Id, 
                PollingStationId = elector.PollingStationId
            };
        }
    }
}
