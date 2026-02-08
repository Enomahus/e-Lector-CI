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
    public class TestDataSeeder(WritableDbContext context, UserManager<UserDao> userManager, IOptions<DataConfiguration> dataConfig) 
        : SeederBase(context, userManager)
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
        #endregion


        #region
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
                        new List<string> { AppConstants.SuperAdminRole}
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
                        new List<string> { AppConstants.OrganismAgentRole}
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
        #endregion
    }
}
