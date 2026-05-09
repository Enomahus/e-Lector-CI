using Application.Features.Common.PollingStation;
using Infrastructure.Persistence.Entities;

namespace Application.Features.PollingStation.GetPollingStation
{
    public class GetPollingStationResponse : PollingStationModel
    {
        public string StationNumber { get; set; } = string.Empty;

        public static GetPollingStationResponse Fromdao(PollingStationDao dao, DateTimeOffset dateNow)
        {
            return new GetPollingStationResponse
            {
                StationNumber = dao.StationNumber,
                Wording = dao.Wording,
                ConstituencyId = dao.ConstituencyId,
                IsActive = dao.DisabledDate is null || dao.DisabledDate > dateNow,
            };
        }
    }
}
