using System.Data;
using Application.Features.Common;
using Application.Models;
using FluentValidation;
using Infrastructure.Persistence.SQLServer.Contexts;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tools.Logging;

namespace Application.Features.PollingStation.GetPollingStations
{
    public class GetPollingStationsQuery : IRequest<Result<PagedList<GetPollingStationsResponse>>>
    {
        public string? Sort { get; set; }
        public string? Order { get; set; }
        public int? PageIndex { get; set; } = 0;
        public int PageSize { get; set; } = 20;
    }

    public class GetPollingStationsQueryValidator : AbstractValidator<GetPollingStationsQuery>
    {
        public GetPollingStationsQueryValidator() { }
    }

    public class GetPollingStationsQueryHandler(ReadOnlyDbContext context, TimeProvider timeProvider)
        : IRequestHandler<GetPollingStationsQuery, Result<PagedList<GetPollingStationsResponse>>>
    {
        public async Task<Result<PagedList<GetPollingStationsResponse>>> Handle(
            GetPollingStationsQuery query,
            CancellationToken cancellationToken
        )
        {
            using var activity = ActivitySourceLog.CQRS.Start();
            var now = timeProvider.GetUtcNow();

            string baseSql =
                @"
            SELECT 
                ps.Id as StationId, ps.StationNumber, ps.Wording as StationWording, ps.DisabledDate,
                c.Id as VotingLocationId, c.Code as VotingLocationCode, c.Wording as VotingLocationName,
                m.Id as MunicipalityId, m.Code as MunicipalityCode, m.Wording as MunicipalityName,
                sp.Id as SubPrefectureId, sp.Code as SubPrefectureCode, sp.Wording as SubPrefectureName,
                d.Id as DepartmentId, d.Code as DepartmentCode, d.Wording as DepartmentName,
                r.Id as RegionId, r.Code as RegionCode, r.Wording as RegionName
            FROM PollingStations ps
            LEFT JOIN Constituencies c ON ps.ConstituencyId = c.Id
            LEFT JOIN Constituencies m ON c.ParentId = m.Id
            LEFT JOIN Constituencies sp ON m.ParentId = sp.Id
            LEFT JOIN Constituencies d ON sp.ParentId = d.Id
            LEFT JOIN Constituencies r ON d.ParentId = r.Id";

            var totalCount = await context.PollingStations.CountAsync(cancellationToken);

            // 3. Gestion sécurisée du tri (Whitelist des colonnes autorisées)
            var allowedSortColumns = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "stationNumber", "ps.StationNumber" },
                { "votingLocationName", "c.Wording" },
                { "municipalityName", "m.Wording" },
                { "regionName", "r.Wording" },
            };

            string sortColumn = allowedSortColumns.GetValueOrDefault(query.Sort ?? "", "ps.StationNumber");
            string sortOrder = query.Order?.ToLower() == "desc" ? "DESC" : "ASC";

            // 4. Construction de la requête finale avec Pagination
            // L'utilisation de OFFSET / FETCH NEXT est le standard SQL moderne pour la pagination
            string finalSql =
                @$"
            {baseSql}
            ORDER BY {sortColumn} {sortOrder}
            OFFSET @Skip ROWS FETCH NEXT @Take ROWS ONLY";

            var skip = query.PageIndex * query.PageSize;
            var take = query.PageSize;

            // 5. Exécution avec paramètres pour éviter les injections
            var rows = await context
                .Database.SqlQueryRaw<FlatPollingStationRow>(
                    finalSql,
                    new Microsoft.Data.SqlClient.SqlParameter("@Skip", skip),
                    new Microsoft.Data.SqlClient.SqlParameter("@Take", take)
                )
                .ToListAsync(cancellationToken);

            // 6. Mapping
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

            return Result<PagedList<GetPollingStationsResponse>>.From(
                new PagedList<GetPollingStationsResponse>(models, totalCount)
            );

            #region to be deleted
            //const string sql =
            //    @"WITH Flattened AS (
            //    SELECT
            //        ps.Id as StationId,
            //        ps.StationNumber,
            //        ps.Wording as StationWording,
            //        ps.DisabledDate,
            //        c.Id as StartId,
            //        c.Wording,
            //        c.Code,
            //        c.Level,
            //        c.ParentId
            //    FROM PollingStations ps
            //    JOIN Constituencies c ON ps.ConstituencyId = c.Id
            //),
            //Recursion AS (
            //    SELECT StationId, StationNumber, StationWording, DisabledDate, ParentId , Wording, Code, Level, StartId, 1 as Depth
            //    FROM Flattened
            //    UNION ALL
            //    SELECT r.StationId, r.StationNumber, r.StationWording, r.DisabledDate,c.ParentId , c.Wording, c.Code, c.Level, r.StartId, r.Depth + 1
            //    FROM Recursion r
            //    JOIN Constituencies c ON r.ParentId = c.Id
            //)

            //SELECT
            //    ps.Id as StationId,
            //    ps.StationNumber,
            //    ps.Wording as StationWording,
            //    ps.DisabledDate,
            //    c.Id as VotingLocationId, c.Code as VotingLocationCode, c.Wording as VotingLocationName,
            //    m.Id as MunicipalityId, m.Code as MunicipalityCode, m.Wording as MunicipalityName,
            //    sp.Id as SubPrefectureId, sp.Code as SubPrefectureCode, sp.Wording as SubPrefectureName,
            //    d.Id as DepartmentId, d.Code as DepartmentCode, d.Wording as DepartmentName,
            //    r.Id as RegionId, r.Code as RegionCode, r.Wording as RegionName
            //FROM PollingStations ps
            //LEFT JOIN Constituencies c ON ps.ConstituencyId = c.Id -- VotingLocation
            //LEFT JOIN Constituencies m ON c.ParentId = m.Id  -- Municipality
            //LEFT JOIN Constituencies sp ON m.ParentId = sp.Id -- SubPrefecture
            //LEFT JOIN Constituencies d ON sp.ParentId = d.Id  -- Department
            //LEFT JOIN Constituencies r ON d.ParentId = r.Id   -- Region";

            //var rows = await context
            //    .Database.SqlQueryRaw<FlatPollingStationRow>(sql)
            //    .ToListAsync(cancellationToken);

            //var models = rows.Select(row => new GetPollingStationsResponse(
            //        row.RegionId,
            //        row.RegionCode,
            //        row.RegionName,
            //        row.DepartmentId,
            //        row.DepartmentCode,
            //        row.DepartmentName,
            //        row.SubPrefectureId,
            //        row.SubPrefectureCode,
            //        row.SubPrefectureName,
            //        row.MunicipalityId,
            //        row.MunicipalityCode,
            //        row.MunicipalityName,
            //        row.VotingLocationId,
            //        row.VotingLocationCode,
            //        row.VotingLocationName,
            //        row.StationId,
            //        row.StationNumber,
            //        row.DisabledDate is null || row.DisabledDate > now
            //    ))
            //    .ToList();

            //return Result<List<GetPollingStationsResponse>>.From(models);
            #endregion
        }
    }
}
