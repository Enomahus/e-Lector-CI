using Application.Features.Common.Constituency;
using Application.Features.Common.PollingStation;
using Infrastructure.Persistence.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Features.PollingStation.GetPollingStations
{
    public class GetPollingStationsByConstituencyIdResponse : PollingStationModel
    {
        public ConstituencyModel? Constituency {  get; set; }

        public static GetPollingStationsByConstituencyIdResponse From(PollingStationDao dao, DateTimeOffset dateNow)
        {
            return new GetPollingStationsByConstituencyIdResponse
            {
                StationNumber = dao.StationNumber,
                Wording = dao.Wording,
                ConstituencyId = dao.ConstituencyId,
                IsActive = dao.DisabledDate is null || dao.DisabledDate > dateNow,
                Constituency = ConstituencyModel.From(dao.Constituency, dateNow)
            };
        }
    }
}
