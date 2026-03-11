using Application.Common.Enums;
using Application.Models;
using Infrastructure.Persistence.Entities;
using MediatR;

namespace Application.Features.Common.Elector
{
    public class ElectorModel : IRequest<Result<Guid>>
    {
        public Guid? Id { get; set; }
        public string? VoterNumber { get; set; }
        public string? LastName { get; set; }

        public string? MarriedName { get; set; }
        public string? FirstNames { get; set; }

        public Gender Gender { get; set; }
        public DateTime DateOfBirth { get; set; }

        public string? PlaceOfBirth { get; set; }

        public string? Profession { get; set; }
        public string? PhysicalAddress { get; set; }
        public string? PostalAddress { get; set; }

        public long? PollingStationId {  get; set; }

        public ElectorDao ToDao(long? pollingStationId)
        {
            return new ElectorDao
            {
                DateOfBirth = DateOfBirth,
                PlaceOfBirth = PlaceOfBirth,
                Profession = Profession,
                PhysicalAddress = PhysicalAddress,
                PostalAddress = PostalAddress,
                LastName = LastName,
                MarriedName = MarriedName,
                FirstNames = FirstNames,
                Gender = Gender,
                PollingStationId = pollingStationId,
                VoterNumber = VoterNumber,
                
            };
        }

        public ElectorDao ToDao(PollingStationDao? pollingStation)
        {
            var elector = ToDao(pollingStation?.Id);
            elector.PollingStation = pollingStation;
            return elector;
        }

        public static ElectorModel From(ElectorDao elector)
        {
            return new ElectorModel
            {
                Id = elector.Id,
                VoterNumber = elector.VoterNumber,
                Gender = elector.Gender,
                LastName = elector.LastName,
                MarriedName = elector.MarriedName,
                FirstNames = elector.FirstNames,
                DateOfBirth = elector.DateOfBirth,
                PlaceOfBirth = elector.PlaceOfBirth,
                Profession = elector.Profession,
                PhysicalAddress = elector.PhysicalAddress,
                PostalAddress = elector.PostalAddress,
                PollingStationId = elector.PollingStationId,
            };
        }
    }
}
