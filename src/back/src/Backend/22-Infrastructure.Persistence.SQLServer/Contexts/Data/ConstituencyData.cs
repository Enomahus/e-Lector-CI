using Application.Common.Enums;
using Infrastructure.Persistence.Entities;

namespace Infrastructure.Persistence.SQLServer.Contexts.Data;

public static class ConstituencyData
{
    public static IEnumerable<ConstituencyDao> GetConstituencies =>
        [
            // 01. District autonome d'Abidjan
            new ConstituencyDao { Id = 1, Code = "001", Wording = "District autonome d'Abidjan", Level = LocationLevel.Region },
            new ConstituencyDao { Id = 2, Code = "001001", ParentId = 1, Wording = "Abidjan", Level = LocationLevel.Department },

            // 02. Agnéby-Tiassa
            new ConstituencyDao { Id = 3, Code = "002", Wording = "Agnéby-Tiassa", Level = LocationLevel.Region },
            new ConstituencyDao { Id = 4, Code = "002001", ParentId = 3, Wording = "Agboville", Level = LocationLevel.Department },
            new ConstituencyDao { Id = 5, Code = "002002", ParentId = 3, Wording = "Sikensi", Level = LocationLevel.Department },
            new ConstituencyDao { Id = 6, Code = "002003", ParentId = 3, Wording = "Taabo", Level = LocationLevel.Department },
            new ConstituencyDao { Id = 7, Code = "002004", ParentId = 3, Wording = "Tiassalé", Level = LocationLevel.Department },

            // 03. Bafing
            new ConstituencyDao { Id = 8, Code = "003", Wording = "Bafing", Level = LocationLevel.Region },
            new ConstituencyDao { Id = 9, Code = "003001", ParentId = 8, Wording = "Koro", Level = LocationLevel.Department },
            new ConstituencyDao { Id = 10, Code = "003002", ParentId = 8, Wording = "Ouaninou", Level = LocationLevel.Department },
            new ConstituencyDao { Id = 11, Code = "003003", ParentId = 8, Wording = "Touba", Level = LocationLevel.Department },

            // 04. Bagoué
            new ConstituencyDao { Id = 12, Code = "004", Wording = "Bagoué", Level = LocationLevel.Region },
            new ConstituencyDao { Id = 13, Code = "004001", ParentId = 12, Wording = "Boundiali", Level = LocationLevel.Department },
            new ConstituencyDao { Id = 14, Code = "004002", ParentId = 12, Wording = "Kouto", Level = LocationLevel.Department },
            new ConstituencyDao { Id = 15, Code = "004003", ParentId = 12, Wording = "Tengréla", Level = LocationLevel.Department },

            // 05. Bélier
            new ConstituencyDao { Id = 16, Code = "005", Wording = "Bélier", Level = LocationLevel.Region },
            new ConstituencyDao { Id = 17, Code = "005001", ParentId = 16, Wording = "Didiévi", Level = LocationLevel.Department },
            new ConstituencyDao { Id = 18, Code = "005002", ParentId = 16, Wording = "Djékanou", Level = LocationLevel.Department },
            new ConstituencyDao { Id = 19, Code = "005003", ParentId = 16, Wording = "Tiébissou", Level = LocationLevel.Department },
            new ConstituencyDao { Id = 20, Code = "005004", ParentId = 16, Wording = "Toumodi", Level = LocationLevel.Department },

            // 06. Béré
            new ConstituencyDao { Id = 21, Code = "006", Wording = "Béré", Level = LocationLevel.Region },
            new ConstituencyDao { Id = 22, Code = "006001", ParentId = 21, Wording = "Dianra", Level = LocationLevel.Department },
            new ConstituencyDao { Id = 23, Code = "006002", ParentId = 21, Wording = "Kounahiri", Level = LocationLevel.Department },
            new ConstituencyDao { Id = 24, Code = "006003", ParentId = 21, Wording = "Mankono", Level = LocationLevel.Department },

            // 07. Bounkani
            new ConstituencyDao { Id = 25, Code = "007", Wording = "Bounkani", Level = LocationLevel.Region },
            new ConstituencyDao { Id = 26, Code = "007001", ParentId = 25, Wording = "Bouna", Level = LocationLevel.Department },
            new ConstituencyDao { Id = 27, Code = "007002", ParentId = 25, Wording = "Doropo", Level = LocationLevel.Department },
            new ConstituencyDao { Id = 28, Code = "007003", ParentId = 25, Wording = "Nassian", Level = LocationLevel.Department },
            new ConstituencyDao { Id = 29, Code = "007004", ParentId = 25, Wording = "Téhini", Level = LocationLevel.Department },

            // 08. Cavally
            new ConstituencyDao { Id = 30, Code = "008", Wording = "Cavally", Level = LocationLevel.Region },
            new ConstituencyDao { Id = 31, Code = "008001", ParentId = 30, Wording = "Bloléquin", Level = LocationLevel.Department },
            new ConstituencyDao { Id = 32, Code = "008002", ParentId = 30, Wording = "Guiglo", Level = LocationLevel.Department },
            new ConstituencyDao { Id = 33, Code = "008003", ParentId = 30, Wording = "Taï", Level = LocationLevel.Department },
            new ConstituencyDao { Id = 34, Code = "008004", ParentId = 30, Wording = "Toulepleu", Level = LocationLevel.Department },

            // 09. Folon
            new ConstituencyDao { Id = 35, Code = "009", Wording = "Folon", Level = LocationLevel.Region },
            new ConstituencyDao { Id = 36, Code = "009001", ParentId = 35, Wording = "Kaniasso", Level = LocationLevel.Department },
            new ConstituencyDao { Id = 37, Code = "009002", ParentId = 35, Wording = "Minignan", Level = LocationLevel.Department },

            // 10. Gbêkê
            new ConstituencyDao { Id = 38, Code = "010", Wording = "Gbêkê", Level = LocationLevel.Region },
            new ConstituencyDao { Id = 39, Code = "010001", ParentId = 38, Wording = "Béoumi", Level = LocationLevel.Department },
            new ConstituencyDao { Id = 40, Code = "010002", ParentId = 38, Wording = "Botro", Level = LocationLevel.Department },
            new ConstituencyDao { Id = 41, Code = "010003", ParentId = 38, Wording = "Bouaké", Level = LocationLevel.Department },
            new ConstituencyDao { Id = 42, Code = "010004", ParentId = 38, Wording = "Sakassou", Level = LocationLevel.Department },

            // 11. Gbôklé
            new ConstituencyDao { Id = 43, Code = "011", Wording = "Gbôklé", Level = LocationLevel.Region },
            new ConstituencyDao { Id = 44, Code = "011001", ParentId = 43, Wording = "Fresco", Level = LocationLevel.Department },
            new ConstituencyDao { Id = 45, Code = "011002", ParentId = 43, Wording = "Sassandra", Level = LocationLevel.Department },

            // 12. Gôh
            new ConstituencyDao { Id = 46, Code = "012", Wording = "Gôh", Level = LocationLevel.Region },
            new ConstituencyDao { Id = 47, Code = "012001", ParentId = 46, Wording = "Gagnoa", Level = LocationLevel.Department },
            new ConstituencyDao { Id = 48, Code = "012002", ParentId = 46, Wording = "Oumé", Level = LocationLevel.Department },

            // 13. Gontougou
            new ConstituencyDao { Id = 49, Code = "013", Wording = "Gontougou", Level = LocationLevel.Region },
            new ConstituencyDao { Id = 50, Code = "013001", ParentId = 49, Wording = "Bondoukou", Level = LocationLevel.Department },
            new ConstituencyDao { Id = 51, Code = "013002", ParentId = 49, Wording = "Koun-Fao", Level = LocationLevel.Department },
            new ConstituencyDao { Id = 52, Code = "013003", ParentId = 49, Wording = "Sandégué", Level = LocationLevel.Department },
            new ConstituencyDao { Id = 53, Code = "013004", ParentId = 49, Wording = "Tanda", Level = LocationLevel.Department },
            new ConstituencyDao { Id = 54, Code = "013005", ParentId = 49, Wording = "Transua", Level = LocationLevel.Department },

            // 14. Grands Ponts
            new ConstituencyDao { Id = 55, Code = "014", Wording = "Grands Ponts", Level = LocationLevel.Region },
            new ConstituencyDao { Id = 56, Code = "014001", ParentId = 55, Wording = "Dabou", Level = LocationLevel.Department },
            new ConstituencyDao { Id = 57, Code = "014002", ParentId = 55, Wording = "Grand-Lahou", Level = LocationLevel.Department },
            new ConstituencyDao { Id = 58, Code = "014003", ParentId = 55, Wording = "Jacqueville", Level = LocationLevel.Department },

            // 15. Guémon
            new ConstituencyDao { Id = 59, Code = "015", Wording = "Guémon", Level = LocationLevel.Region },
            new ConstituencyDao { Id = 60, Code = "015001", ParentId = 59, Wording = "Bangolo", Level = LocationLevel.Department },
            new ConstituencyDao { Id = 61, Code = "015002", ParentId = 59, Wording = "Duékoué", Level = LocationLevel.Department },
            new ConstituencyDao { Id = 62, Code = "015003", ParentId = 59, Wording = "Facobly", Level = LocationLevel.Department },
            new ConstituencyDao { Id = 63, Code = "015004", ParentId = 59, Wording = "Kouibly", Level = LocationLevel.Department },

            // 16. Haut-Sassandra
            new ConstituencyDao { Id = 64, Code = "016", Wording = "Haut-Sassandra", Level = LocationLevel.Region },
            new ConstituencyDao { Id = 65, Code = "016001", ParentId = 64, Wording = "Daloa", Level = LocationLevel.Department },
            new ConstituencyDao { Id = 66, Code = "016002", ParentId = 64, Wording = "Issia", Level = LocationLevel.Department },
            new ConstituencyDao { Id = 67, Code = "016003", ParentId = 64, Wording = "Vavoua", Level = LocationLevel.Department },
            new ConstituencyDao { Id = 68, Code = "016004", ParentId = 64, Wording = "Zoukougbeu", Level = LocationLevel.Department },

            // 17. Iffou
            new ConstituencyDao { Id = 69, Code = "017", Wording = "Iffou", Level = LocationLevel.Region },
            new ConstituencyDao { Id = 70, Code = "017001", ParentId = 69, Wording = "Daoukro", Level = LocationLevel.Department },
            new ConstituencyDao { Id = 71, Code = "017002", ParentId = 69, Wording = "M’Bahiakro", Level = LocationLevel.Department },
            new ConstituencyDao { Id = 72, Code = "017003", ParentId = 69, Wording = "Ouellé", Level = LocationLevel.Department },
            new ConstituencyDao { Id = 73, Code = "017004", ParentId = 69, Wording = "Prikro", Level = LocationLevel.Department },

            // 18. Indénié-Djuablin
            new ConstituencyDao { Id = 74, Code = "018", Wording = "Indénié-Djuablin", Level = LocationLevel.Region },
            new ConstituencyDao { Id = 75, Code = "018001", ParentId = 74, Wording = "Abengourou", Level = LocationLevel.Department },
            new ConstituencyDao { Id = 76, Code = "018002", ParentId = 74, Wording = "Agnibilékrou", Level = LocationLevel.Department },
            new ConstituencyDao { Id = 77, Code = "018003", ParentId = 74, Wording = "Bettié", Level = LocationLevel.Department },

            // 19. Kabadougou
            new ConstituencyDao { Id = 78, Code = "019", Wording = "Kabadougou", Level = LocationLevel.Region },
            new ConstituencyDao { Id = 79, Code = "019001", ParentId = 78, Wording = "Gbéléban", Level = LocationLevel.Department },
            new ConstituencyDao { Id = 80, Code = "019002", ParentId = 78, Wording = "Madinani", Level = LocationLevel.Department },
            new ConstituencyDao { Id = 81, Code = "019003", ParentId = 78, Wording = "Odienné", Level = LocationLevel.Department },
            new ConstituencyDao { Id = 82, Code = "019004", ParentId = 78, Wording = "Samatiguila", Level = LocationLevel.Department },
            new ConstituencyDao { Id = 83, Code = "019005", ParentId = 78, Wording = "Séguélon", Level = LocationLevel.Department },

            // 20. La Mé
            new ConstituencyDao { Id = 84, Code = "020", Wording = "La Mé", Level = LocationLevel.Region },
            new ConstituencyDao { Id = 85, Code = "020001", ParentId = 84, Wording = "Adzopé", Level = LocationLevel.Department },
            new ConstituencyDao { Id = 86, Code = "020002", ParentId = 84, Wording = "Akoupé", Level = LocationLevel.Department },
            new ConstituencyDao { Id = 87, Code = "020003", ParentId = 84, Wording = "Alépé", Level = LocationLevel.Department },
            new ConstituencyDao { Id = 88, Code = "020004", ParentId = 84, Wording = "Yakassé-Attobrou", Level = LocationLevel.Department },

            // 21. Lôh-Djiboua
            new ConstituencyDao { Id = 89, Code = "021", Wording = "Lôh-Djiboua", Level = LocationLevel.Region },
            new ConstituencyDao { Id = 90, Code = "021001", ParentId = 89, Wording = "Divo", Level = LocationLevel.Department },
            new ConstituencyDao { Id = 91, Code = "021002", ParentId = 89, Wording = "Guitry", Level = LocationLevel.Department },
            new ConstituencyDao { Id = 92, Code = "021003", ParentId = 89, Wording = "Lakota", Level = LocationLevel.Department },

            // 22. Marahoué
            new ConstituencyDao { Id = 93, Code = "022", Wording = "Marahoué", Level = LocationLevel.Region },
            new ConstituencyDao { Id = 94, Code = "022001", ParentId = 93, Wording = "Bonon", Level = LocationLevel.Department },
            new ConstituencyDao { Id = 95, Code = "022002", ParentId = 93, Wording = "Bouaflé", Level = LocationLevel.Department },
            new ConstituencyDao { Id = 96, Code = "022003", ParentId = 93, Wording = "Gohitafla", Level = LocationLevel.Department },
            new ConstituencyDao { Id = 97, Code = "022004", ParentId = 93, Wording = "Sinfra", Level = LocationLevel.Department },
            new ConstituencyDao { Id = 98, Code = "022005", ParentId = 93, Wording = "Zuénoula", Level = LocationLevel.Department },

            // 23. Hambol
            new ConstituencyDao { Id = 99, Code = "023", Wording = "Hambol", Level = LocationLevel.Region },
            new ConstituencyDao { Id = 100, Code = "023001", ParentId = 99, Wording = "Dabakala", Level = LocationLevel.Department },
            new ConstituencyDao { Id = 101, Code = "023002", ParentId = 99, Wording = "Katiola", Level = LocationLevel.Department },
            new ConstituencyDao { Id = 102, Code = "023003", ParentId = 99, Wording = "Niakaramadougou", Level = LocationLevel.Department },

            // 24. Moronou
            new ConstituencyDao { Id = 103, Code = "024", Wording = "Moronou", Level = LocationLevel.Region },
            new ConstituencyDao { Id = 104, Code = "024001", ParentId = 103, Wording = "Arrah", Level = LocationLevel.Department },
            new ConstituencyDao { Id = 105, Code = "024002", ParentId = 103, Wording = "Bongouanou", Level = LocationLevel.Department },
            new ConstituencyDao { Id = 106, Code = "024003", ParentId = 103, Wording = "M’Batto", Level = LocationLevel.Department },

            // 25. Nawa
            new ConstituencyDao { Id = 107, Code = "025", Wording = "Nawa", Level = LocationLevel.Region },
            new ConstituencyDao { Id = 108, Code = "025001", ParentId = 107, Wording = "Buyo", Level = LocationLevel.Department },
            new ConstituencyDao { Id = 109, Code = "025002", ParentId = 107, Wording = "Guéyo", Level = LocationLevel.Department },
            new ConstituencyDao { Id = 110, Code = "025003", ParentId = 107, Wording = "Méagui", Level = LocationLevel.Department },
            new ConstituencyDao { Id = 111, Code = "025004", ParentId = 107, Wording = "Soubré", Level = LocationLevel.Department },

            // 26. N'Zi
            new ConstituencyDao { Id = 112, Code = "026", Wording = "N'Zi", Level = LocationLevel.Region },
            new ConstituencyDao { Id = 113, Code = "026001", ParentId = 112, Wording = "Bocanda", Level = LocationLevel.Department },
            new ConstituencyDao { Id = 114, Code = "026002", ParentId = 112, Wording = "Dimbokro", Level = LocationLevel.Department },
            new ConstituencyDao { Id = 115, Code = "026003", ParentId = 112, Wording = "Kouassi-Kouassikro", Level = LocationLevel.Department },

            // 27. Poro
            new ConstituencyDao { Id = 116, Code = "027", Wording = "Poro", Level = LocationLevel.Region },
            new ConstituencyDao { Id = 117, Code = "027001", ParentId = 116, Wording = "Dikodougou", Level = LocationLevel.Department },
            new ConstituencyDao { Id = 118, Code = "027002", ParentId = 116, Wording = "Korhogo", Level = LocationLevel.Department },
            new ConstituencyDao { Id = 119, Code = "027003", ParentId = 116, Wording = "M’Bengué", Level = LocationLevel.Department },
            new ConstituencyDao { Id = 120, Code = "027004", ParentId = 116, Wording = "Sinématiali", Level = LocationLevel.Department },

            // 28. San-Pédro
            new ConstituencyDao { Id = 121, Code = "028", Wording = "San-Pédro", Level = LocationLevel.Region },
            new ConstituencyDao { Id = 122, Code = "028001", ParentId = 121, Wording = "San-Pédro", Level = LocationLevel.Department },
            new ConstituencyDao { Id = 123, Code = "028002", ParentId = 121, Wording = "Tabou", Level = LocationLevel.Department },

            // 29. Sud-Comoé
            new ConstituencyDao { Id = 124, Code = "029", Wording = "Sud-Comoé", Level = LocationLevel.Region },
            new ConstituencyDao { Id = 125, Code = "029001", ParentId = 124, Wording = "Aboisso", Level = LocationLevel.Department },
            new ConstituencyDao { Id = 126, Code = "029002", ParentId = 124, Wording = "Adiaké", Level = LocationLevel.Department },
            new ConstituencyDao { Id = 127, Code = "029003", ParentId = 124, Wording = "Grand-Bassam", Level = LocationLevel.Department },
            new ConstituencyDao { Id = 128, Code = "029004", ParentId = 124, Wording = "Tiapoum", Level = LocationLevel.Department },

            // 30. Tchologo
            new ConstituencyDao { Id = 129, Code = "030", Wording = "Tchologo", Level = LocationLevel.Region },
            new ConstituencyDao { Id = 130, Code = "030001", ParentId = 129, Wording = "Ferkessédougou", Level = LocationLevel.Department },
            new ConstituencyDao { Id = 131, Code = "030002", ParentId = 129, Wording = "Kong", Level = LocationLevel.Department },
            new ConstituencyDao { Id = 132, Code = "030003", ParentId = 129, Wording = "Ouangolodougou", Level = LocationLevel.Department },

            // 31. Tonkpi
            new ConstituencyDao { Id = 133, Code = "031", Wording = "Tonkpi", Level = LocationLevel.Region },
            new ConstituencyDao { Id = 134, Code = "031001", ParentId = 133, Wording = "Biankouma", Level = LocationLevel.Department },
            new ConstituencyDao { Id = 135, Code = "031002", ParentId = 133, Wording = "Danané", Level = LocationLevel.Department },
            new ConstituencyDao { Id = 136, Code = "031003", ParentId = 133, Wording = "Man", Level = LocationLevel.Department },
            new ConstituencyDao { Id = 137, Code = "031004", ParentId = 133, Wording = "Sipilou", Level = LocationLevel.Department },
            new ConstituencyDao { Id = 138, Code = "031005", ParentId = 133, Wording = "Zouan-Hounien", Level = LocationLevel.Department },

            // 32. Worodougou
            new ConstituencyDao { Id = 139, Code = "032", Wording = "Worodougou", Level = LocationLevel.Region },
            new ConstituencyDao { Id = 140, Code = "032001", ParentId = 139, Wording = "Kani", Level = LocationLevel.Department },
            new ConstituencyDao { Id = 141, Code = "032002", ParentId = 139, Wording = "Séguéla", Level = LocationLevel.Department },

            // 33. District autonome de Yamoussoukro
            new ConstituencyDao { Id = 142, Code = "033", Wording = "District autonome de Yamoussoukro", Level = LocationLevel.Region },
            new ConstituencyDao { Id = 143, Code = "033001", ParentId = 142, Wording = "Attiégouakro", Level = LocationLevel.Department },
            new ConstituencyDao { Id = 144, Code = "033002", ParentId = 142, Wording = "Yamoussoukro", Level = LocationLevel.Department },

            // 34. Diaspora
            new ConstituencyDao { Id = 145, Code = "099", Wording = "Diaspora", Level = LocationLevel.Region },


            new ConstituencyDao { Id =146, Code = "005001001", Wording = "Boli"         , ParentId = 17 , Level = LocationLevel.SubPrefecture },
            new ConstituencyDao { Id =147, Code = "005001002", Wording = "Didiévi"      , ParentId = 17 , Level = LocationLevel.SubPrefecture },
            new ConstituencyDao { Id =148, Code = "005001003", Wording = "Molonou-blé"  , ParentId = 17 , Level = LocationLevel.SubPrefecture },
            new ConstituencyDao { Id =149, Code = "005001004", Wording = "Raviart"      , ParentId = 17 , Level = LocationLevel.SubPrefecture },
            new ConstituencyDao { Id =150, Code = "005001005", Wording = "Tié-N'diekro" , ParentId = 17 , Level = LocationLevel.SubPrefecture },

            new ConstituencyDao { Id =151, Code = "005001001001", Wording = "Boli", ParentId = 146 , Level = LocationLevel.Municipality },

            new ConstituencyDao { Id =152, Code = "005001001001001", Wording = "EPP Allanikro"        , ParentId = 151 , Level = LocationLevel.VotingLocation },
            new ConstituencyDao { Id =153, Code = "005001001001002", Wording = "EPP Anokoi-Kouamekro" , ParentId = 151 , Level = LocationLevel.VotingLocation },
            new ConstituencyDao { Id =154, Code = "005001001001003", Wording = "EPP Labo"             , ParentId = 151 , Level = LocationLevel.VotingLocation },
            new ConstituencyDao { Id =155, Code = "005001001001004", Wording = "EPP Adjebo"           , ParentId = 151 , Level = LocationLevel.VotingLocation },
            new ConstituencyDao { Id =156, Code = "005001001001005", Wording = "EPP Takikro"          , ParentId = 151 , Level = LocationLevel.VotingLocation },
            new ConstituencyDao { Id =157, Code = "005001001001006", Wording = "EPP Aka Kouamekro"    , ParentId = 151 , Level = LocationLevel.VotingLocation },
            new ConstituencyDao { Id =158, Code = "005001001001007", Wording = "EPP Kongobo"          , ParentId = 151 , Level = LocationLevel.VotingLocation },
            new ConstituencyDao { Id =159, Code = "005001001001008", Wording = "EPP Boli 1"           , ParentId = 151 , Level = LocationLevel.VotingLocation },
            new ConstituencyDao { Id =160, Code = "005001001001009", Wording = "EPP Boli 3"           , ParentId = 151 , Level = LocationLevel.VotingLocation },
            new ConstituencyDao { Id =161, Code = "005001001001010", Wording = "EPP Yoboueplissou"    , ParentId = 151 , Level = LocationLevel.VotingLocation },
            new ConstituencyDao { Id =162, Code = "005001001001011", Wording = "EPP Anokoi-Djezou"    , ParentId = 151 , Level = LocationLevel.VotingLocation },
            new ConstituencyDao { Id =163, Code = "005001001001012", Wording = "EPP Grodiekro"        , ParentId = 151 , Level = LocationLevel.VotingLocation },

        ];
}
