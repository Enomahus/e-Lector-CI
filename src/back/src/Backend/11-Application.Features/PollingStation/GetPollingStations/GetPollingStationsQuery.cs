using System.Data;
using Application.Models;
using Application.Models.Errors;
using FluentValidation;
using Infrastructure.Persistence.Entities;
using Infrastructure.Persistence.SQLServer.Contexts;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tools.Logging;

namespace Application.Features.PollingStation.GetPollingStations
{
    public class GetPollingStationsQuery : IRequest<Result<List<GetPollingStationsResponse>>> { }

    public class GetPollingStationsByConstituencyIdQueryValidator : AbstractValidator<GetPollingStationsQuery>
    {
        public GetPollingStationsByConstituencyIdQueryValidator() { }
    }

    public class GetPollingStationsByConstituencyIdQueryHandler(
        ReadOnlyDbContext context,
        TimeProvider timeProvider
    ) : IRequestHandler<GetPollingStationsQuery, Result<List<GetPollingStationsResponse>>>
    {
        public async Task<Result<List<GetPollingStationsResponse>>> Handle(
            GetPollingStationsQuery query,
            CancellationToken cancellationToken
        )
        {
            using var activity = ActivitySourceLog.CQRS.Start();
            var now = timeProvider.GetUtcNow();

            const string sql =
                @"WITH Flattened AS (
                SELECT 
                    ps.Id as StationId,
                    ps.StationNumber,
                    ps.Wording as StationWording,
                    ps.DisabledDate,
                    c.Id as StartId,
                    c.Wording,
                    c.Code,
                    c.Level,
                    c.ParentId 
                FROM PollingStations ps
                JOIN Constituencies c ON ps.ConstituencyId = c.Id
            ),
            Recursion AS (
                SELECT StationId, StationNumber, StationWording, DisabledDate, ParentId , Wording, Code, Level, StartId, 1 as Depth
                FROM Flattened
                UNION ALL
                SELECT r.StationId, r.StationNumber, r.StationWording, r.DisabledDate,c.ParentId , c.Wording, c.Code, c.Level, r.StartId, r.Depth + 1
                FROM Recursion r
                JOIN Constituencies c ON r.ParentId = c.Id
            )

            SELECT 
                ps.Id as StationId,
                ps.StationNumber,
                ps.Wording as StationWording,
                ps.DisabledDate,
                c.Id as VotingLocationId, c.Code as VotingLocationCode, c.Wording as VotingLocationName,
                m.Id as MunicipalityId, m.Code as MunicipalityCode, m.Wording as MunicipalityName,
                sp.Id as SubPrefectureId, sp.Code as SubPrefectureCode, sp.Wording as SubPrefectureName,
                d.Id as DepartmentId, d.Code as DepartmentCode, d.Wording as DepartmentName,
                r.Id as RegionId, r.Code as RegionCode, r.Wording as RegionName
            FROM PollingStations ps
            LEFT JOIN Constituencies c ON ps.ConstituencyId = c.Id -- VotingLocation
            LEFT JOIN Constituencies m ON c.ParentId = m.Id  -- Municipality
            LEFT JOIN Constituencies sp ON m.ParentId = sp.Id -- SubPrefecture
            LEFT JOIN Constituencies d ON sp.ParentId = d.Id  -- Department
            LEFT JOIN Constituencies r ON d.ParentId = r.Id   -- Region";

            var rows = await context
                .Database.SqlQueryRaw<FlatPollingStationRow>(sql)
                .ToListAsync(cancellationToken);

            var models = rows.Select(row => new GetPollingStationsResponse(
                    row.RegionId,
                    row.RegionCode,
                    row.RegionName,
                    row.DepartmentId,
                    row.DepartmentCode,
                    row.DepartmentName,
                    row.SubPrefectureId,
                    row.SubPrefectureCode,
                    row.SubPrefectureName,
                    row.MunicipalityId,
                    row.MunicipalityCode,
                    row.MunicipalityName,
                    row.VotingLocationId,
                    row.VotingLocationCode,
                    row.VotingLocationName,
                    row.StationId,
                    row.StationNumber,
                    row.DisabledDate is null || row.DisabledDate > now
                ))
                .ToList();

            return Result<List<GetPollingStationsResponse>>.From(models);
        }
    }
}
