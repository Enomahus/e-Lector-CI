using System.Data;
using Application.Common.Enums;
using Application.Features.Common;
using Application.Models;
using FluentValidation;
using Infrastructure.Persistence.SQLServer.Contexts;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Pcea.Core.Net.Authorization.Application.Attributes;
using Tools.Logging;

namespace Application.Features.PollingStation.GetPollingStations
{
    [WithPermission(nameof(AppPermission.GetPollingStations))]
    public class GetPollingStationsQuery : IRequest<Result<PagedList<GetPollingStationsResponse>>>
    {
        public string? Sort { get; set; }
        public string? Order { get; set; }
        public int? PageIndex { get; set; } = 0;
        public int PageSize { get; set; } = 20;
        public string? Search { get; set; }
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

            try
            {
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

                string whereClause = "";
                var sqlParams = new List<Microsoft.Data.SqlClient.SqlParameter>();
                if (!string.IsNullOrWhiteSpace(query.Search))
                {
                    whereClause =
                        @"
                WHERE 
                    ps.StationNumber LIKE @Search OR
                    c.Wording LIKE @Search OR
                    m.Wording LIKE @Search OR
                    sp.Wording LIKE @Search OR
                    d.Wording LIKE @Search OR
                    r.Wording LIKE @Search";
                    sqlParams.Add(
                        new Microsoft.Data.SqlClient.SqlParameter("@Search", SqlDbType.NVarChar)
                        {
                            Value = $"%{query.Search}%",
                        }
                    );
                }

                string countSql =
                    $@"SELECT COUNT(*) AS [Value] FROM PollingStations ps 
                            LEFT JOIN Constituencies c ON ps.ConstituencyId = c.Id 
                            LEFT JOIN Constituencies m ON c.ParentId = m.Id
                            LEFT JOIN Constituencies sp ON m.ParentId = sp.Id
                            LEFT JOIN Constituencies d ON sp.ParentId = d.Id
                            LEFT JOIN Constituencies r ON d.ParentId = r.Id
                            {whereClause}";

                var totalCount = await context
                    .Database.SqlQueryRaw<int>(
                        countSql,
                        sqlParams.Select(p => ((ICloneable)p).Clone()).ToArray()
                    )
                    .FirstAsync(cancellationToken);

                // 3. Gestion sécurisée du tri (Whitelist des colonnes autorisées)
                var allowedSortColumns = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                {
                    { "stationNumber", "ps.StationNumber" },
                    { "votingLocationName", "c.Wording" },
                    { "municipalityName", "m.Wording" },
                    { "subPrefectureName", "sp.Wording" },
                    { "departmentName", "d.Wording" },
                    { "regionName", "r.Wording" },
                };

                string sortColumn = allowedSortColumns.GetValueOrDefault(
                    query.Sort ?? "",
                    "ps.StationNumber"
                );
                string sortOrder = query.Order?.ToLower() == "desc" ? "DESC" : "ASC";

                string finalSql =
                    @$"
                    {baseSql}
                    {whereClause}
                    ORDER BY {sortColumn} {sortOrder}
                    OFFSET @Skip ROWS FETCH NEXT @Take ROWS ONLY";

                var finalParams = sqlParams
                    .Select(p => ((ICloneable)p).Clone())
                    .Cast<Microsoft.Data.SqlClient.SqlParameter>()
                    .ToList();
                finalParams.Add(
                    new Microsoft.Data.SqlClient.SqlParameter(
                        "@Skip",
                        (query.PageIndex ?? 0) * query.PageSize
                    )
                );
                finalParams.Add(new Microsoft.Data.SqlClient.SqlParameter("@Take", query.PageSize));

                // 5. Exécution avec paramètres pour éviter les injections
                var rows = await context
                    .Database.SqlQueryRaw<FlatPollingStationRow>(finalSql, [.. finalParams])
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
                        row.DisabledDate is null || row.DisabledDate > now,
                        row.DisabledDate
                    ))
                    .ToList();

                return Result<PagedList<GetPollingStationsResponse>>.From(
                    new PagedList<GetPollingStationsResponse>(models, totalCount)
                );
            }
            catch (OperationCanceledException ex)
            {
                activity?.SetException(ex);
                throw;
            }
            catch (Exception ex)
            {
                activity?.SetException(ex);
                return Result<PagedList<GetPollingStationsResponse>>.From();
            }
        }
    }
}
