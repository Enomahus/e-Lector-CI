using Application.Common.Enums;
using Infrastructure.Persistence.Entities;

namespace Infrastructure.Persistence.SQLServer.Contexts.Data;

public static class ConstituencyData
{
    public static IEnumerable<ConstituencyDao> Constituencies =>
        [
            new()
            {
                Id = 1,
                Code = "A0",
                Name = "DIAPORA",
                Level = LocationLevel.Region,
            },
            new()
            {
                Id = 2,
                Code = "A1",
                Name = "DISTRICT AUTONOME D'ABIDJAN",
                Level = LocationLevel.Region,
            },
            new()
            {
                Id = 3,
                Code = "A2",
                Name = "DISTRICT AUTONOME DE YAMOUSSOUKRO",
                Level = LocationLevel.Region,
            },
            new()
            {
                Id = 4,
                Code = "10",
                Name = "BELIER",
                Level = LocationLevel.Region,
            },
            new()
            {
                Id = 5,
                Code = "010",
                Name = "DIDIEVI",
                Level = LocationLevel.Department,
                ParentId = 4,
            },
            new()
            {
                Id = 6,
                Code = "011",
                Name = "TIEBISSOU",
                Level = LocationLevel.Department,
                ParentId = 4,
            },
            new()
            {
                Id = 7,
                Code = "012",
                Name = "TOUMODI",
                Level = LocationLevel.Department,
                ParentId = 4,
            },
            new()
            {
                Id = 8,
                Code = "096",
                Name = "DJEKANOU",
                Level = LocationLevel.Department,
                ParentId = 4,
            },
            new()
            {
                Id = 9,
                Code = "002",
                Name = "DIDIEVI",
                Level = LocationLevel.SubPrefecture,
                ParentId = 5,
            },
            new()
            {
                Id = 10,
                Code = "003",
                Name = "TIE-N'DIEKRO",
                Level = LocationLevel.SubPrefecture,
                ParentId = 5,
            },
            new()
            {
                Id = 11,
                Code = "004",
                Name = "BOLI",
                Level = LocationLevel.SubPrefecture,
                ParentId = 5,
            },
            new()
            {
                Id = 12,
                Code = "005",
                Name = "MOLONOU-BLE",
                Level = LocationLevel.SubPrefecture,
                ParentId = 5,
            },
            new()
            {
                Id = 13,
                Code = "006",
                Name = "RAVIART",
                Level = LocationLevel.SubPrefecture,
                ParentId = 5,
            },
            new()
            {
                Id = 14,
                Code = "001",
                Name = "TIEBISSOU",
                Level = LocationLevel.SubPrefecture,
                ParentId = 6,
            },
            new()
            {
                Id = 15,
                Code = "002",
                Name = "LOMOKANKRO",
                Level = LocationLevel.SubPrefecture,
                ParentId = 6,
            },
            new()
            {
                Id = 16,
                Code = "003",
                Name = "MOLONOU",
                Level = LocationLevel.SubPrefecture,
                ParentId = 6,
            },
            new()
            {
                Id = 17,
                Code = "004",
                Name = "YAKPABO-SAKASSOU",
                Level = LocationLevel.SubPrefecture,
                ParentId = 6,
            },
            new()
            {
                Id = 18,
                Code = "001",
                Name = "ANGONDA",
                Level = LocationLevel.SubPrefecture,
                ParentId = 7,
            },
            new()
            {
                Id = 19,
                Code = "003",
                Name = "KOKUMBO",
                Level = LocationLevel.SubPrefecture,
                ParentId = 7,
            },
            new()
            {
                Id = 20,
                Code = "004",
                Name = "KPOUEBO",
                Level = LocationLevel.SubPrefecture,
                ParentId = 7,
            },
            new()
            {
                Id = 21,
                Code = "005",
                Name = "TOUMODI",
                Level = LocationLevel.SubPrefecture,
                ParentId = 7,
            },
            new()
            {
                Id = 22,
                Code = "001",
                Name = "BONIKRO",
                Level = LocationLevel.SubPrefecture,
                ParentId = 8,
            },
            new()
            {
                Id = 23,
                Code = "002",
                Name = "DJEKANOU",
                Level = LocationLevel.SubPrefecture,
                ParentId = 8,
            },
            new()
            {
                Id = 24,
                Code = "098",
                Name = "BOLI",
                Level = LocationLevel.Municipality,
                ParentId = 11,
            },
            new()
            {
                Id = 25,
                Code = "001",
                Name = "EPP ALLANIKRO",
                Level = LocationLevel.VotingLocation,
                ParentId = 24,
            },
            new()
            {
                Id = 26,
                Code = "002",
                Name = "EPP ANOKOI-KOUAMEKRO",
                Level = LocationLevel.VotingLocation,
                ParentId = 24,
            },
            new()
            {
                Id = 27,
                Code = "003",
                Name = "EPP LABO",
                Level = LocationLevel.VotingLocation,
                ParentId = 24,
            },
            new()
            {
                Id = 28,
                Code = "004",
                Name = "EPP ADJEBO",
                Level = LocationLevel.VotingLocation,
                ParentId = 24,
            },
            new()
            {
                Id = 29,
                Code = "005",
                Name = "EPP TAKIKRO",
                Level = LocationLevel.VotingLocation,
                ParentId = 24,
            },
            new()
            {
                Id = 30,
                Code = "006",
                Name = "EPP AKA KOUAMEKRO",
                Level = LocationLevel.VotingLocation,
                ParentId = 24,
            },
            new()
            {
                Id = 31,
                Code = "007",
                Name = "EPP KONGOBO",
                Level = LocationLevel.VotingLocation,
                ParentId = 24,
            },
            new()
            {
                Id = 32,
                Code = "008",
                Name = "EPP BOLI 1",
                Level = LocationLevel.VotingLocation,
                ParentId = 24,
            },
            new()
            {
                Id = 33,
                Code = "009",
                Name = "EPP BOLI 3",
                Level = LocationLevel.VotingLocation,
                ParentId = 24,
            },
            new()
            {
                Id = 34,
                Code = "010",
                Name = "EPP YOBOUEPLISSOU",
                Level = LocationLevel.VotingLocation,
                ParentId = 24,
            },
            new()
            {
                Id = 35,
                Code = "011",
                Name = "EPP ANOKOI-DJEZOU",
                Level = LocationLevel.VotingLocation,
                ParentId = 24,
            },
            new()
            {
                Id = 36,
                Code = "012",
                Name = "EPP GRODIEKRO",
                Level = LocationLevel.VotingLocation,
                ParentId = 24,
            },
        ];
}
