using Infrastructure.Persistence.Entities;

namespace Application.Features.Common.PollingStation
{
    public class PollingStationModel
    {
        public string? StationNumber { get; set; }
        public string? Wording { get; set; }
        public long ConstituencyId { get; set; }
        public bool IsActive { get; set; }

        public static PollingStationModel FromDao(PollingStationDao dao, DateTimeOffset dateNow)
        {
            return new PollingStationModel
            {
                StationNumber = dao.StationNumber,
                Wording = dao.Wording,
                ConstituencyId = dao.ConstituencyId,
                IsActive = dao.DisabledDate is null || dao.DisabledDate > dateNow
            };
        }

        public PollingStationDao ToDao()
        {
            return new PollingStationDao()
            {
                StationNumber = StationNumber!,
                Wording = Wording!,
                ConstituencyId = ConstituencyId
            };
        }
    }
}
