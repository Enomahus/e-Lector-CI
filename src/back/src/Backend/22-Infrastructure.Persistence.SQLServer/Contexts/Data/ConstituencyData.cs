using Application.Common.Enums;
using Infrastructure.Persistence.Entities;

namespace Infrastructure.Persistence.SQLServer.Contexts.Data;

public static class ConstituencyData
{
    public static IEnumerable<ConstituencyDao> GetConstituencies =>
        [
            new ConstituencyDao { Id =1, Code = "RG01", Wording = "Agnéby-Tiassa", Level = LocationLevel.Region },
            new ConstituencyDao { Id =2, Code = "RG02", Wording = "Bafing", Level = LocationLevel.Region },
            new ConstituencyDao { Id =3, Code = "RG03", Wording = "Bagoué", Level = LocationLevel.Region },
            new ConstituencyDao { Id =4, Code = "RG04", Wording = "Bélier", Level = LocationLevel.Region },
            new ConstituencyDao { Id =5, Code = "RG05", Wording = "Béré", Level = LocationLevel.Region },
            new ConstituencyDao { Id =6, Code = "RG06", Wording = "Bounkani", Level = LocationLevel.Region },
            new ConstituencyDao { Id =7, Code = "RG07", Wording = "Cavally", Level = LocationLevel.Region },
            new ConstituencyDao { Id =8, Code = "RG08", Wording = "Folon", Level = LocationLevel.Region },
            new ConstituencyDao { Id =9, Code = "RG09", Wording = "Gbêkê", Level = LocationLevel.Region },
            new ConstituencyDao { Id =10, Code = "RG10", Wording = "Gbôklé", Level = LocationLevel.Region },
            new ConstituencyDao { Id =11, Code = "RG11", Wording = "Gôh", Level = LocationLevel.Region },
            new ConstituencyDao { Id =12, Code = "RG12", Wording = "Gontougo", Level = LocationLevel.Region },
            new ConstituencyDao { Id =13, Code = "RG13", Wording = "Grands-Ponts", Level = LocationLevel.Region },
            new ConstituencyDao { Id =14, Code = "RG14", Wording = "Guémon", Level = LocationLevel.Region },
            new ConstituencyDao { Id =15, Code = "RG15", Wording = "Hambol", Level = LocationLevel.Region },
            new ConstituencyDao { Id =16, Code = "RG16", Wording = "Haut-Sassandra", Level = LocationLevel.Region },
            new ConstituencyDao { Id =17, Code = "RG17", Wording = "Iffou", Level = LocationLevel.Region },
            new ConstituencyDao { Id =18, Code = "RG18", Wording = "Indénié-Djuablin", Level = LocationLevel.Region },
            new ConstituencyDao { Id =19, Code = "RG19", Wording = "Kabadougou", Level = LocationLevel.Region },
            new ConstituencyDao { Id =20, Code = "RG20", Wording = "La Mé", Level = LocationLevel.Region },
            new ConstituencyDao { Id =21, Code = "RG21", Wording = "Lôh-Djiboua", Level = LocationLevel.Region },
            new ConstituencyDao { Id =22, Code = "RG22", Wording = "Marahoué", Level = LocationLevel.Region },
            new ConstituencyDao { Id =23, Code = "RG23", Wording = "Moronou", Level = LocationLevel.Region },
            new ConstituencyDao { Id =24, Code = "RG24", Wording = "Nawa", Level = LocationLevel.Region },
            new ConstituencyDao { Id =25, Code = "RG25", Wording = "N'Zi", Level = LocationLevel.Region },
            new ConstituencyDao { Id =26, Code = "RG26", Wording = "Poro", Level = LocationLevel.Region },
            new ConstituencyDao { Id =27, Code = "RG27", Wording = "San-Pédro", Level = LocationLevel.Region },
            new ConstituencyDao { Id =28, Code = "RG28", Wording = "Sud-Comoé", Level = LocationLevel.Region },
            new ConstituencyDao { Id =29, Code = "RG29", Wording = "Tchologo", Level = LocationLevel.Region },
            new ConstituencyDao { Id =30, Code = "RG30", Wording = "Tonkpi", Level = LocationLevel.Region },
            new ConstituencyDao { Id =31, Code = "RG31", Wording = "Worodougou", Level = LocationLevel.Region },
            new ConstituencyDao { Id =32, Code = "RG32", Wording = "District autonome d'Abidjan", Level = LocationLevel.Region },
            new ConstituencyDao { Id =33, Code = "RG33", Wording = "District autonome de Yamoussoukro", Level = LocationLevel.Region },
            new ConstituencyDao { Id =99, Code = "RG99", Wording = "Diapora", Level = LocationLevel.Region },

            new ConstituencyDao { Id =34, Code = "R04DE010", Wording = "Didiévi", ParentId = 4 , Level = LocationLevel.Department },
            new ConstituencyDao { Id =35, Code = "R04DE011", Wording = "Djékanou", ParentId = 4 , Level = LocationLevel.Department },
            new ConstituencyDao { Id =36, Code = "R04DE012", Wording = "Tiébissou", ParentId = 4 , Level = LocationLevel.Department },
            new ConstituencyDao { Id =37, Code = "R04DE013", Wording = "Toumodi", ParentId = 4 , Level = LocationLevel.Department },

            new ConstituencyDao { Id =38, Code = "R04SB038", Wording = "Boli", ParentId = 34 , Level = LocationLevel.SubPrefecture },
            new ConstituencyDao { Id =39, Code = "R04SB039", Wording = "Didiévi", ParentId = 34 , Level = LocationLevel.SubPrefecture },
            new ConstituencyDao { Id =40, Code = "R04SB040", Wording = "Molonou-blé", ParentId = 34 , Level = LocationLevel.SubPrefecture },
            new ConstituencyDao { Id =41, Code = "R04SB041", Wording = "Raviart", ParentId = 34 , Level = LocationLevel.SubPrefecture },
            new ConstituencyDao { Id =42, Code = "R04SB042", Wording = "Tié-N'diekro", ParentId = 34 , Level = LocationLevel.SubPrefecture },

            new ConstituencyDao { Id =43, Code = "R04CM043", Wording = "Boli", ParentId = 38 , Level = LocationLevel.Municipality },

            new ConstituencyDao { Id =44, Code = "R04VL001", Wording = "EPP Allanikro", ParentId = 43 , Level = LocationLevel.VotingLocation },
            new ConstituencyDao { Id =45, Code = "R04VL002", Wording = "EPP Anokoi-Kouamekro", ParentId = 43 , Level = LocationLevel.VotingLocation },
            new ConstituencyDao { Id =46, Code = "R04VL003", Wording = "EPP Labo", ParentId = 43 , Level = LocationLevel.VotingLocation },
            new ConstituencyDao { Id =47, Code = "R04VL004", Wording = "EPP Adjebo", ParentId = 43 , Level = LocationLevel.VotingLocation },
            new ConstituencyDao { Id =48, Code = "R04VL005", Wording = "EPP Takikro", ParentId = 43 , Level = LocationLevel.VotingLocation },
            new ConstituencyDao { Id =49, Code = "R04VL006", Wording = "EPP Aka Kouamekro", ParentId = 43 , Level = LocationLevel.VotingLocation },
            new ConstituencyDao { Id =50, Code = "R04VL007", Wording = "EPP Kongobo", ParentId = 43 , Level = LocationLevel.VotingLocation },
            new ConstituencyDao { Id =51, Code = "R04VL008", Wording = "EPP Boli 1", ParentId = 43 , Level = LocationLevel.VotingLocation },
            new ConstituencyDao { Id =52, Code = "R04VL009", Wording = "EPP Boli 3", ParentId = 43 , Level = LocationLevel.VotingLocation },
            new ConstituencyDao { Id =53, Code = "R04VL010", Wording = "EPP Yoboueplissou", ParentId = 43 , Level = LocationLevel.VotingLocation },
            new ConstituencyDao { Id =54, Code = "R04VL011", Wording = "EPP Anokoi-Djezou", ParentId = 43 , Level = LocationLevel.VotingLocation },
            new ConstituencyDao { Id =55, Code = "R04VL012", Wording = "EPP Grodiekro", ParentId = 43 , Level = LocationLevel.VotingLocation },

        ];
}
