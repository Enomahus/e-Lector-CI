using Infrastructure.Persistence.Configurations;
using Infrastructure.Persistence.Entities;
using Infrastructure.Persistence.SQLServer.Contexts;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Options;
using Tools.Constants;

namespace Infrastructure.Persistence.SQLServer.Seeders
{
    public class TestDataSeeder(
        WritableDbContext context,
        UserManager<UserDao> userManager,
        IOptions<DataConfiguration> dataConfig
    ) : SeederBase(context, userManager)
    {
        public override async Task SeedDataAsync()
        {
            if (!dataConfig.Value.SeedTest)
            {
                return;
            }
            var strategy = _context.Database.CreateExecutionStrategy();
            await strategy.ExecuteInTransactionAsync(
                async () =>
                {
                    await SeedUsersAsync();
                    await SeedPollingStationAsync();
                },
                () => Task.FromResult(true)
            );
        }

        #region Seeds
        private async Task SeedUsersAsync()
        {
            var users = GetMockUsers();

            foreach (var user in users)
            {
                if (!_context.Users.Any(u => u.UserName == user.Item1.UserName))
                    await SeedUserAsync(user.Item1, "Secret01", user.Item2);
            }

            await _context.SaveChangesAsync();
        }

        private async Task SeedPollingStationAsync()
        {
            var pollingStations = GetMockPollingStations();
            foreach (var station in pollingStations)
            {
                var dbStation = await _context.PollingStations.FirstOrDefaultAsync(s =>
                    s.Wording == station.Wording && s.StationNumber == station.StationNumber
                );
                if (dbStation == null)
                {
                    await _context.PollingStations.AddAsync(station);
                }
                else
                {
                    dbStation.StationNumber = station.StationNumber;
                    _context.PollingStations.Update(dbStation);
                }
            }
            await _context.SaveChangesAsync();
        }
        #endregion


        #region Mock
        public static IEnumerable<Tuple<UserDao, List<string>>> GetMockUsers()
        {
            return
            [
                Tuple.Create(
                    new UserDao()
                    {
                        UserName = "admin",
                        FirstName = "John",
                        LastName = "Doe",
                        Email = "john.doe@pcea.com",
                        PhoneNumber = "01 02 03 04 05",
                    },
                    new List<string> { AppConstants.SuperAdminRole }
                ),
                Tuple.Create(
                    new UserDao()
                    {
                        UserName = "user1",
                        FirstName = "Sam",
                        LastName = "Gamegie",
                        Email = "sam.gamegie@pcea.com",
                        PhoneNumber = "01 02 03 04 05",
                    },
                    new List<string> { AppConstants.OrganismAgentRole }
                ),
                Tuple.Create(
                    new UserDao()
                    {
                        UserName = "user2",
                        FirstName = "Bilbo",
                        LastName = "Baggins",
                        PhoneNumber = "01 02 03 04 05",
                        Email = "bilbo.baggins@pcea.com",
                    },
                    new List<string> { AppConstants.ElectorRole }
                ),
                Tuple.Create(
                    new UserDao()
                    {
                        UserName = "user3",
                        FirstName = "Éowyn",
                        LastName = "Shieldmaiden",
                        PhoneNumber = "01 02 03 04 05",
                        Email = "eowyn.shieldmaiden@pcea.com",
                    },
                    new List<string> { AppConstants.ElectorRole }
                ),
                Tuple.Create(
                    new UserDao()
                    {
                        UserName = "user4",
                        FirstName = "Éomer",
                        LastName = "RiderOfRohan",
                        PhoneNumber = "01 02 03 04 05",
                        Email = "eomer.riderofrohan@pcea.com",
                    },
                    new List<string> { AppConstants.OrganismAgentRole }
                ),
            ];
        }

        public static IEnumerable<PollingStationDao> GetMockPollingStations()
        {
            return
            [
                new PollingStationDao()
                {
                    StationNumber = "1",
                    Wording = "EPP Allanikro",
                    ConstituencyId = 152,
                },
                new PollingStationDao()
                {
                    StationNumber = "2",
                    Wording = "EPP Allanikro",
                    ConstituencyId = 152,
                },
                new PollingStationDao()
                {
                    StationNumber = "1",
                    Wording = "EPP Anokoi-Kouamekro",
                    ConstituencyId = 153,
                },
                new PollingStationDao()
                {
                    StationNumber = "1",
                    Wording = "EPP Labo",
                    ConstituencyId = 154,
                },
                new PollingStationDao()
                {
                    StationNumber = "1",
                    Wording = "EPP Adjebo",
                    ConstituencyId = 155,
                },
                new PollingStationDao()
                {
                    StationNumber = "2",
                    Wording = "EPP Adjebo",
                    ConstituencyId = 155,
                },
                new PollingStationDao()
                {
                    StationNumber = "1",
                    Wording = "EPP Takikro",
                    ConstituencyId = 156,
                },
                new PollingStationDao()
                {
                    StationNumber = "1",
                    Wording = "EPP Aka Kouamekro",
                    ConstituencyId = 157,
                },
                new PollingStationDao()
                {
                    StationNumber = "1",
                    Wording = "EPP Kongobo",
                    ConstituencyId = 158,
                },
                new PollingStationDao()
                {
                    StationNumber = "1",
                    Wording = "EPP Boli 1",
                    ConstituencyId = 159,
                },
                new PollingStationDao()
                {
                    StationNumber = "2",
                    Wording = "EPP Boli 1",
                    ConstituencyId = 159,
                },
                new PollingStationDao()
                {
                    StationNumber = "1",
                    Wording = "EPP Boli 3",
                    ConstituencyId = 160,
                },
                new PollingStationDao()
                {
                    StationNumber = "2",
                    Wording = "EPP Boli 3",
                    ConstituencyId = 160,
                },
                new PollingStationDao()
                {
                    StationNumber = "3",
                    Wording = "EPP Boli 3",
                    ConstituencyId = 160,
                },
                new PollingStationDao()
                {
                    StationNumber = "1",
                    Wording = "EPP Yoboueplissou",
                    ConstituencyId = 161,
                },
                new PollingStationDao()
                {
                    StationNumber = "1",
                    Wording = "EPP Anokoi-Djezou",
                    ConstituencyId = 162,
                },
                new PollingStationDao()
                {
                    StationNumber = "2",
                    Wording = "EPP Anokoi-Djezou",
                    ConstituencyId = 162,
                },
                new PollingStationDao()
                {
                    StationNumber = "1",
                    Wording = "EPP Grodiekro",
                    ConstituencyId = 163,
                },
                new PollingStationDao()
                {
                    StationNumber = "2",
                    Wording = "EPP Grodiekro",
                    ConstituencyId = 163,
                },
            ];
        }
        #endregion
    }
}
