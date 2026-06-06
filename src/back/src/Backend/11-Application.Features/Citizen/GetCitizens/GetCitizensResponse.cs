using Application.Common.Enums;
using Infrastructure.Persistence.Entities;

namespace Application.Features.Citizen.GetCitizens
{
    public class GetCitizensResponse
    {
        public Guid Id { get; set; }
        public Gender Gender { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public DateTimeOffset BirthDate { get; set; }
        public required string BirthPlace { get; set; }
        public required string Nationality { get; set; }

        public static GetCitizensResponse FromDao(CitizenDao dao)
        {
            return new GetCitizensResponse()
            {
                Id = dao.Id,
                Gender = dao.Gender,
                FirstName = dao.FirstName,
                LastName = dao.LastName,
                BirthDate = dao.BirthDate,
                BirthPlace = dao.BirthPlace,
                Nationality = dao.Nationality,
            };
        }
    }
}
