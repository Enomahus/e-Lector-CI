using Application.Common.Enums;
using Application.Features.Common.PollingStation;
using Infrastructure.Persistence.Entities;
using System.Data;

namespace Application.Features.Constituency.GetConstituencies
{
    public class GetConstituenciesResponse
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Wording { get; set; }
        public LocationLevel Level { get; set; }
        public ICollection<GetConstituenciesResponse> Children { get; set; } = [];
        public ICollection<PollingStationModel> PollingStations { get; set; } = [];


        public static GetConstituenciesResponse From(ConstituencyDao dao, DateTimeOffset now)
        {
            return new GetConstituenciesResponse()
            {
                Code = dao.Code,
                Id = dao.Id,
                Level = dao.Level,
                Wording = dao.Wording,
                Children = dao.Subconstituency?.Select(x => From(x, now)).ToList() ?? [],
                PollingStations = dao.PollingStations?.Select(ps => PollingStationModel.FromDao(ps, now)).ToList() ?? [],
            };
        }
    }
    
}
