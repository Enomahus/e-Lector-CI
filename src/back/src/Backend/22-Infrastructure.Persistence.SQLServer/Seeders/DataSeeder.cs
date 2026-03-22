using Application.Common.Enums;
using Infrastructure.Persistence.Configurations;
using Infrastructure.Persistence.Entities;
using Infrastructure.Persistence.SQLServer.Contexts;
using Infrastructure.Persistence.SQLServer.Contexts.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Tools.Constants;
using Tools.Exceptions;

namespace Infrastructure.Persistence.SQLServer.Seeders;

public class DataSeeder(WritableDbContext context, UserManager<UserDao> userManager,
    RoleManager<RoleDao> roleManager, IOptions<DataConfiguration> dataConfig) : SeederBase(context, userManager)
{
    private static readonly string AdminUserName = "pcea_admin";

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
                    await SeedConstituenciesAsync();
                },
                () => Task.FromResult(true)
            );
    }

    private async Task SeedConstituenciesAsync()
    {
        var constituencies = ConstituencyData.GetConstituencies;
        foreach (var constituency in constituencies)
        {
            var dbConstituency = await _context.Constituencies
                .FirstOrDefaultAsync(c => c.Wording == constituency.Wording && c.Level == constituency.Level);

            if (dbConstituency == null) 
            {
                await _context.Constituencies.AddAsync(constituency);
            }
            else
            {
                dbConstituency.Code = constituency.Code;

                _context.Constituencies.Update(dbConstituency);
            }
        }
        await _context.SaveChangesAsync();
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
        // First, update the list of permissions and actions
        var newPermissions = Enum.GetValues<AppPermission>();
        var existingPermissionsArray = string.Join(", ", newPermissions.Select(p => $"'{p.ToString()}'"));
        var deleteSqlCommand =
            $"DELETE FROM AppPermission WHERE PermissionCode NOT IN ({existingPermissionsArray})";
        if (_context.Database.IsRelational())
        {
            await _context.Database.ExecuteSqlRawAsync(deleteSqlCommand);
        }
        var existingPermissions = await _context.AppPermissions.ToListAsync();
        _context.AddRange(
            newPermissions
                .Where(pNew => !existingPermissions.Any(pEx => pEx.PermissionCode == pNew))
                .Select(perm => new AppPermissionDao() { PermissionCode = perm })
        );

        var newActions = Enum.GetValues<AppAction>().Select(perm => new AppActionDao() { ActionCode = perm });
        var existingActionsArray = string.Join(", ", newActions.Select(p => $"'{p.ToString()}'"));
        deleteSqlCommand = $"DELETE FROM AppAction WHERE ActionCode NOT IN ({existingActionsArray})";
        if (_context.Database.IsRelational())
        {
            await _context.Database.ExecuteSqlRawAsync(deleteSqlCommand);
        }
        var existingActions = await _context.AppActions.ToListAsync();
        _context.AddRange(
            newActions.Where(pNew => !existingActions.Any(pEx => pEx.ActionCode == pNew.ActionCode))
        );
        await _context.SaveChangesAsync();
        _context.ChangeTracker.Clear();

        // Then, we update the links between the roles, actions and permissions
        var allActions = await _context.AppActions.Include(a => a.Permissions).ToListAsync();
        var allPermissions = await _context.AppPermissions.ToListAsync();

        foreach (var action in RolesData.ActionsSeed)
        {
            var actionDao = allActions.First(a => a.ActionCode == action.Key);
            var correspondingPermissions = allPermissions
                .Where(p => action.Value.Contains(p.PermissionCode))
                .ToList();

            foreach (
                var perm in correspondingPermissions.Where(cp =>
                    !actionDao.Permissions.Any(p => p.Id == cp.Id)
                )
            )
            {
                actionDao.Permissions.Add(perm);
            }
            foreach (
                var perm in actionDao.Permissions.Where(p =>
                    !correspondingPermissions.Any(cp => p.Id == cp.Id)
                )
            )
            {
                actionDao.Permissions.Remove(perm);
            }
        }

        foreach (var role in RolesData.RolesSeed)
        {
            if (!await _context.Roles.AnyAsync(r => r.Name == role.Key))
            {
                await roleManager.CreateAsync(new RoleDao(role.Key));
            }
            var roleDao = await _context.Roles.Include(r => r.Actions).FirstAsync(r => r.Name == role.Key);

            //if (Enum.TryParse<ActivityCode>(role.Key, out var activityCode))
            //{
            //    roleDao.ActivityId = await _context
            //        .Activities.Where(r => r.ActivityCode == activityCode)
            //        .Select(a => a.Id)
            //        .SingleAsync();
            //}

            var correspondingActions = allActions.Where(a => role.Value.Contains(a.ActionCode)).ToList();

            foreach (var perm in correspondingActions.Where(ca => !roleDao.Actions.Any(a => a.Id == ca.Id)))
            {
                roleDao.Actions.Add(perm);
            }
            foreach (var perm in roleDao.Actions.Where(a => !correspondingActions.Any(ca => a.Id == ca.Id)))
            {
                roleDao.Actions.Remove(perm);
            }
        }
        await _context.SaveChangesAsync();

    }
}
