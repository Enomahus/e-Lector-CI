using Application.Common.Enums;
using Infrastructure.Persistence.Entities;

namespace Infrastructure.Persistence.SQLServer.Contexts.Data;

public static class GeographicAreaData
{
    public static IEnumerable<GeographicAreaDao> GeographicAreas =>
        [
            new() 
            {
                Id = 1,
                Name = "AFRIQUE",
                Level = LocationLevel.Continent
            },
            new()
            {
                Id = 2,
                Name = "AMERIQUE DU NORD",
                Level = LocationLevel.Continent
            },
            new()
            {
                Id = 3,
                Name = "AMERIQUE DU SUD",
                Level = LocationLevel.Continent
            },
            new()
            {
                Id = 4,
                Name = "ANTARCTIQUE",
                Level = LocationLevel.Continent
            },
            new()
            {
                Id = 5,
                Name = "ASIE",
                Level = LocationLevel.Continent
            },
            new()
            {
                Id = 6,
                Name = "EUROPE",
                Level = LocationLevel.Continent
            },
            new()
            {
                Id = 7,
                Name = "OCEANIE",
                Level = LocationLevel.Continent
            },
            new()
            {
                Id = 8,
                Name = "COTE D'IVOIRE",
                Level = LocationLevel.Country,
                ParentId = 1
            },
        ];
}
