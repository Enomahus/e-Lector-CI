using Application.Common.Enums;
using Application.Features.Common.PollingStation;

namespace Application.Features.Constituency.GetConstituencies
{
    public record GetConstituenciesResponse(
        long Id,
        string Code,
        string Wording,
        LocationLevel Level,
        ICollection<GetConstituenciesResponse> SubConstituencies
        //ICollection<PollingStationModel> PollingStation
    );
}
