using Application.Common.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using Tools.Constants;

namespace Infrastructure.Persistence.SQLServer.Contexts.Data
{
    public static class RolesData
    {
        public static readonly Dictionary<AppAction, List<AppPermission>> ActionsSeed = new()
        {
            { AppAction.SuperAdmin, Enum.GetValues<AppPermission>().ToList() },
        };

        public static readonly Dictionary<string, List<AppAction>> RolesSeed = new()
        {
            { AppConstants.SuperAdminRole, [AppAction.SuperAdmin] },
            {
                AppConstants.OrganismAgentRole,
                [
                    AppAction.Default,
                    AppAction.RegistrationRequestConsultation,
                    AppAction.RegistrationRequestsManagement,
                ]
            },
            {
                AppConstants.ElectorRole,
                [
                    AppAction.Default,
                    AppAction.RegistrationRequestCreation,
                    AppAction.RegistrationRequestConsultation,
                ]
            }
        };
    }
}
