using Application.Models.Auth;
using Infrastructure.Persistence.Entities;
using Infrastructure.Persistence.SQLServer.Contexts;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces.Services
{
    public interface ITokenService
    {
        void TrackRefreshTokensToClean();

        Task<string> CreateRefreshTokenAsync(Guid userId, CancellationToken cancellationToken = default);

        Task<Tokens> CreateTokensAsync(
            UserDao user,
            IEnumerable<string> roles,
            IDictionary<long, IEnumerable<string>> userEntitiesRoles,
            CancellationToken cancellationToken = default
        );

        Task<Tokens> CreateTokensAsync(
            UserDao user,
            UserDao impersonatorUser,
            IEnumerable<string> roles,
            IDictionary<long, IEnumerable<string>> userEntitiesRoles,
            CancellationToken cancellationToken = default
        );
    }
}
