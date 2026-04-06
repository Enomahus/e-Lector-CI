using Application.Exceptions.Auth;
using Application.Interfaces.Services;
using Infrastructure.Persistence.Entities;
using Infrastructure.Persistence.SQLServer.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Security.Common
{
    public class TokenHelper(ITokenService tokenService,ReadOnlyDbContext context) : ITokenHelper
    {        

        public async Task<UserDao> GetUserForAuthenticationAsync(string userName)
        {
            return await context.Users.IncludeUser()
                .Where(u => u.UserName == userName)
                .FirstOrDefaultAsync()
                ?? throw new UserAuthenticationException(userName);
        }

        public async Task<UserDao> GetUserForAuthenticationByIdAsync(Guid userId)
        {
            return await context
                .Users.IncludeUser()
                .Where((u) => u.Id == userId)
                .FirstOrDefaultAsync() ?? throw new UserAuthenticationException(userId!);
        }

        public async Task<TokenResponse> GenerateTokenAsync(UserDao user, CancellationToken cancellationToken)
        {
            var userRolesId = user.UserRoles.Select(ur => ur.Role.Id.ToString());
            var userEntitiesRoles = user.UserConstituencies.ToDictionary(
                ue => ue.ConstituencyId,
                ue => ue.SpecificRoles.Select(r => r.Id.ToString())
            );
            var tokens = await tokenService.CreateTokensAsync(user,userRolesId,userEntitiesRoles,cancellationToken);
            var model = new TokenResponse(tokens.AccessToken,tokens.RefreshToken);
            return model;
        }
    }
}
