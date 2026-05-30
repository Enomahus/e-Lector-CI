using System;
using System.Collections.Generic;
using System.Text;
using Application.Common.Enums;
using Application.Models;
using FluentValidation;
using Infrastructure.Persistence.SQLServer.Contexts;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tools.Logging;

namespace Application.Features.Dashboard.GetDashboardStats
{
    public class GetDashboardStatsQuery : IRequest<Result<GetDashboardStatsResponse>> { }

    public class GetDashboardStatsQueryValidator : AbstractValidator<GetDashboardStatsQuery>
    {
        public GetDashboardStatsQueryValidator()
        {
            // No parameters to validate for this query, but the class is defined for consistency and future extensibility.
        }
    }

    public class GetDashboardStatsQueryHandler(ReadOnlyDbContext context, TimeProvider timeProvider)
        : IRequestHandler<GetDashboardStatsQuery, Result<GetDashboardStatsResponse>>
    {
        public async Task<Result<GetDashboardStatsResponse>> Handle(
            GetDashboardStatsQuery request,
            CancellationToken cancellationToken
        )
        {
            using var activity = ActivitySourceLog.CQRS.Start();
            var dateNow = timeProvider.GetUtcNow();

            var populations = await context
                .Citizens.AsNoTracking()
                .AsSplitQuery()
                .Include(c => c.ElectorProfil.PollingStation.Constituency)
                .Select(i => new
                {
                    i.Gender,
                    Region = $"{i.ElectorProfil.PollingStation.Constituency.Code} {i.ElectorProfil.PollingStation.Constituency.Wording}",
                    Age = dateNow.Year - i.BirthDate.Year,
                })
                .ToListAsync(cancellationToken);

            long totalPopulation = populations.Count;
            if (totalPopulation == 0)
            {
                return Result<GetDashboardStatsResponse>.From(
                    new GetDashboardStatsResponse(0, new GenderStats(0, 0, 0, 0), [], [])
                );
            }

            var menCount = populations.Count(p => p.Gender == Gender.M);
            var womenCount = totalPopulation - menCount;

            var genderOverview = new GenderStats(
                MenCount: menCount,
                MenPercentage: Math.Round((double)menCount / totalPopulation * 100, 2),
                WomenCount: womenCount,
                WomenPercentage: Math.Round((double)womenCount / totalPopulation * 100, 2)
            );

            var populationByRegion = populations
                .GroupBy(p => p.Region)
                .Select(g => new RegionGenderStatDto(
                    Region: g.Key,
                    MenCount: g.Count(p => p.Gender == Gender.M),
                    WomenCount: g.Count(p => p.Gender == Gender.F)
                ))
                .OrderBy(r => r.Region)
                .ToList();

            var ageRangeStats = populations
                .GroupBy(i =>
                    i.Age switch
                    {
                        <= 30 => "<= 30 ans",
                        > 30 and <= 50 => "31-50 ans",
                        > 50 and <= 70 => "51-70 ans",
                        _ => "> 70 ans et plus",
                    }
                )
                .Select(g => new AgeRangeStatDto(
                    RangeLabel: g.Key,
                    Count: g.Count(),
                    Percentage: Math.Round((double)g.Count() / totalPopulation * 100, 2)
                ))
                .OrderBy(r =>
                    r.RangeLabel switch
                    {
                        "<= 30 ans" => 1,
                        "31-50 ans" => 2,
                        "51-70 ans" => 3,
                        _ => 4,
                    }
                )
                .ToList();

            return Result<GetDashboardStatsResponse>.From(
                new GetDashboardStatsResponse(
                    totalPopulation,
                    genderOverview,
                    populationByRegion,
                    ageRangeStats
                )
            );
        }
    }
}
