using Application.Models;
using Infrastructure.Persistence.Entities;
using MediatR;

namespace Application.Features.Common.Elector
{
    public class ElectorModel : IRequest<Result<Guid>>
    {
        public Guid? Id { get; set; }
        public string? VoterRegistrationNumber { get; set; }       
        public DateTimeOffset? RegistrationDate { get; set; }
        public Guid? CitizenId { get; set; }
        
        public ElectorDao ToDao(long? pollingStationId, Guid? citizenId)
        {
            return new ElectorDao
            {                
                VoterRegistrationNumber = VoterRegistrationNumber,
                RegistrationDate = RegistrationDate!.Value,
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
                Id = elector.Id,
                VoterRegistrationNumber = elector.VoterRegistrationNumber,
                RegistrationDate = elector.RegistrationDate,
                CitizenId = elector.Citizen.Id,
                
            };
        }
    }
}
