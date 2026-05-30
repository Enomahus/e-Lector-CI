using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Features.Dashboard.GetDashboardStats
{
    public record GetDashboardStatsResponse(
        long TotalPopulation,
        GenderStats GenderStats,
        List<RegionGenderStatDto> RegionGenderStats,
        List<AgeRangeStatDto> AgeRangeStats
    );

    public record GenderStats(long MenCount, double MenPercentage, long WomenCount, double WomenPercentage);

    public record RegionGenderStatDto(string Region, long MenCount, long WomenCount);

    public record AgeRangeStatDto(string RangeLabel, long Count, double Percentage);
}
