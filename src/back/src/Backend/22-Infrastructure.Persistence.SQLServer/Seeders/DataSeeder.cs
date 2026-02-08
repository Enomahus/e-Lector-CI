using Infrastructure.Persistence.Configurations;
using Infrastructure.Persistence.Entities;
using Infrastructure.Persistence.SQLServer.Contexts;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
//using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Options;
using Tools.Constants;
using Tools.Exceptions;

namespace Infrastructure.Persistence.SQLServer.Seeders;

public class DataSeeder(WritableDbContext context, UserManager<UserDao> userManager,
    RoleManager<RoleDao> roleManager, IOptions<DataConfiguration> dataConfig) : SeederBase(context, userManager)
{
    private static readonly string AdminUserName = "apollo_admin";

    public override async Task SeedDataAsync()
    {
        if(!dataConfig.Value.Seed)
        {
            return;
        }
        var strategy = _context.Database.CreateExecutionStrategy();
        await strategy.ExecuteInTransactionAsync(
                async () =>
                {
                    await SeedDefaultUserAsync();
                    await SeedRolesAsync();
                },
                () => Task.FromResult(true)
            );
    }

    private async Task SeedDefaultUserAsync()
    {
        if(dataConfig.Value.DefaultUserPassword is null)
        {
            throw new ConfigurationMissingException("Missing configuration : DataConfig.DefaultUserConfig");
        }
        await SeedUserAsync(AdminUserName,"Pcea", "Admin", "dev@pcea.com", dataConfig.Value.DefaultUserPassword, [AppConstants.SuperAdminRole]);
    }

    private async Task SeedRolesAsync()
    {
        List<string> globalRoles = [AppConstants.SuperAdminRole, AppConstants.OrganismAgentRole, AppConstants.ElectorRole];
        foreach( var role in globalRoles )
        {
            if(!await _context.Roles.AnyAsync(r => r.Name == role))
            {
                await roleManager.CreateAsync(new RoleDao(role));
            }
        }

        //var activities = await _context.Activities.ToListAsync();
        //foreach (var activity in activities)
        //{
        //    if (!await _context.Roles.AnyAsync(r => r.ActivityId == activity.Id))
        //    {
        //        await roleManager.CreateAsync(new RoleDao(activity.ActivityCode.ToString(), activity.Id));
        //    }
        //}
    }
}
