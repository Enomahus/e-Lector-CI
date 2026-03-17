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
                Wording = "DIAPORA",
                Level = LocationLevel.Region,
            },
            new()
            {
                Id = 2,
                Code = "A1",
                Wording = "DISTRICT AUTONOME D'ABIDJAN",
                Level = LocationLevel.Region,
            },
            new()
            {
                Id = 3,
                Code = "A2",
                Wording = "DISTRICT AUTONOME DE YAMOUSSOUKRO",
                Level = LocationLevel.Region,
            },
            new()
            {
                Id = 4,
                Code = "10",
                Wording = "BELIER",
                Level = LocationLevel.Region,
            },
            new()
            {
                Id = 5,
                Code = "010",
                Wording = "DIDIEVI",
                Level = LocationLevel.Department,
                ParentId = 4,
            },
            new()
            {
                Id = 6,
                Code = "011",
                Wording = "TIEBISSOU",
                Level = LocationLevel.Department,
                ParentId = 4,
            },
            new()
            {
                Id = 7,
                Code = "012",
                Wording = "TOUMODI",
                Level = LocationLevel.Department,
                ParentId = 4,
            },
            new()
            {
                Id = 8,
                Code = "096",
                Wording = "DJEKANOU",
                Level = LocationLevel.Department,
                ParentId = 4,
            },
            new()
            {
                Id = 9,
                Code = "002",
                Wording = "DIDIEVI",
                Level = LocationLevel.SubPrefecture,
                ParentId = 5,
            },
            new()
            {
                Id = 10,
                Code = "003",
                Wording = "TIE-N'DIEKRO",
                Level = LocationLevel.SubPrefecture,
                ParentId = 5,
            },
            new()
            {
                Id = 11,
                Code = "004",
                Wording = "BOLI",
                Level = LocationLevel.SubPrefecture,
                ParentId = 5,
            },
            new()
            {
                Id = 12,
                Code = "005",
                Wording = "MOLONOU-BLE",
                Level = LocationLevel.SubPrefecture,
                ParentId = 5,
            },
            new()
            {
                Id = 13,
                Code = "006",
                Wording = "RAVIART",
                Level = LocationLevel.SubPrefecture,
                ParentId = 5,
            },
            new()
            {
                Id = 14,
                Code = "001",
                Wording = "TIEBISSOU",
                Level = LocationLevel.SubPrefecture,
                ParentId = 6,
            },
            new()
            {
                Id = 15,
                Code = "002",
                Wording = "LOMOKANKRO",
                Level = LocationLevel.SubPrefecture,
                ParentId = 6,
            },
            new()
            {
                Id = 16,
                Code = "003",
                Wording = "MOLONOU",
                Level = LocationLevel.SubPrefecture,
                ParentId = 6,
            },
            new()
            {
                Id = 17,
                Code = "004",
                Wording = "YAKPABO-SAKASSOU",
                Level = LocationLevel.SubPrefecture,
                ParentId = 6,
            },
            new()
            {
                Id = 18,
                Code = "001",
                Wording = "ANGONDA",
                Level = LocationLevel.SubPrefecture,
                ParentId = 7,
            },
            new()
            {
                Id = 19,
                Code = "003",
                Wording = "KOKUMBO",
                Level = LocationLevel.SubPrefecture,
                ParentId = 7,
            },
            new()
            {
                Id = 20,
                Code = "004",
                Wording = "KPOUEBO",
                Level = LocationLevel.SubPrefecture,
                ParentId = 7,
            },
            new()
            {
                Id = 21,
                Code = "005",
                Wording = "TOUMODI",
                Level = LocationLevel.SubPrefecture,
                ParentId = 7,
            },
            new()
            {
                Id = 22,
                Code = "001",
                Wording = "BONIKRO",
                Level = LocationLevel.SubPrefecture,
                ParentId = 8,
            },
            new()
            {
                Id = 23,
                Code = "002",
                Wording = "DJEKANOU",
                Level = LocationLevel.SubPrefecture,
                ParentId = 8,
            },
            new()
            {
                Id = 24,
                Code = "098",
                Wording = "BOLI",
                Level = LocationLevel.Municipality,
                ParentId = 11,
            },
            new()
            {
                Id = 25,
                Code = "001",
                Wording = "EPP ALLANIKRO",
                Level = LocationLevel.VotingLocation,
                ParentId = 24,
            },
            new()
            {
                Id = 26,
                Code = "002",
                Wording = "EPP ANOKOI-KOUAMEKRO",
                Level = LocationLevel.VotingLocation,
                ParentId = 24,
            },
            new()
            {
                Id = 27,
                Code = "003",
                Wording = "EPP LABO",
                Level = LocationLevel.VotingLocation,
                ParentId = 24,
            },
            new()
            {
                Id = 28,
                Code = "004",
                Wording = "EPP ADJEBO",
                Level = LocationLevel.VotingLocation,
                ParentId = 24,
            },
            new()
            {
                Id = 29,
                Code = "005",
                Wording = "EPP TAKIKRO",
                Level = LocationLevel.VotingLocation,
                ParentId = 24,
            },
            new()
            {
                Id = 30,
                Code = "006",
                Wording = "EPP AKA KOUAMEKRO",
                Level = LocationLevel.VotingLocation,
                ParentId = 24,
            },
            new()
            {
                Id = 31,
                Code = "007",
                Wording = "EPP KONGOBO",
                Level = LocationLevel.VotingLocation,
                ParentId = 24,
            },
            new()
            {
                Id = 32,
                Code = "008",
                Wording = "EPP BOLI 1",
                Level = LocationLevel.VotingLocation,
                ParentId = 24,
            },
            new()
            {
                Id = 33,
                Code = "009",
                Wording = "EPP BOLI 3",
                Level = LocationLevel.VotingLocation,
                ParentId = 24,
            },
            new()
            {
                Id = 34,
                Code = "010",
                Wording = "EPP YOBOUEPLISSOU",
                Level = LocationLevel.VotingLocation,
                ParentId = 24,
            },
            new()
            {
                Id = 35,
                Code = "011",
                Wording = "EPP ANOKOI-DJEZOU",
                Level = LocationLevel.VotingLocation,
                ParentId = 24,
            },
            new()
            {
                Id = 36,
                Code = "012",
                Wording = "EPP GRODIEKRO",
                Level = LocationLevel.VotingLocation,
                ParentId = 24,
            },
        ];
}
