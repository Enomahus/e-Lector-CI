namespace Application.Features.PollingStation.GetPollingStations
{
    public record GetPollingStationsResponse(
        long? RegionId,
        string RegionCode,
        string RegionName,
        long? DepartmentId,
        string DepartmentCode,
        string DepartmentName,
        long? SubPrefectureId,
        string SubPrefectureCode,
        string SubPrefectureName,
        long? MunicipalityId,
        string MunicipalityCode,
        string MunicipalityName,
        long? VotingLocationId,
        string VotingLocationCode,
        string VotingLocationName,
        long StataiontId,
        string StationNumber,
        bool IsDisabled
    );

    public class FlatPollingStationRow
    {
        public long StationId { get; set; }
        public string StationNumber { get; set; }
        public string StationWording { get; set; }
        public DateTimeOffset? DisabledDate { get; set; }

        public long? RegionId { get; set; }
        public string RegionCode { get; set; }
        public string RegionName { get; set; }

        public long? DepartmentId { get; set; }
        public string DepartmentCode { get; set; }
        public string DepartmentName { get; set; }

        public long? SubPrefectureId { get; set; }
        public string SubPrefectureCode { get; set; }
        public string SubPrefectureName { get; set; }

        public long? MunicipalityId { get; set; }
        public string MunicipalityCode { get; set; }
        public string MunicipalityName { get; set; }

        public long VotingLocationId { get; set; }
        public string VotingLocationCode { get; set; }
        public string VotingLocationName { get; set; }
    }
}
