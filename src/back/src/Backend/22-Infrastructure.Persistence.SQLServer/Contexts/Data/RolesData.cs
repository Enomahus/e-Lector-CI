using Application.Common.Enums;
using Tools.Constants;

namespace Infrastructure.Persistence.SQLServer.Contexts.Data
{
    public static class RolesData
    {
        public static readonly Dictionary<AppAction, List<AppPermission>> ActionsSeed = new()
        {
            { AppAction.SuperAdmin, Enum.GetValues<AppPermission>().ToList() },
            { 
                AppAction.UsersAdministration,
                [
                    AppPermission.AccessUsersAdminPage,
                    AppPermission.GetUser,
                    AppPermission.GetUsers,
                    AppPermission.CreateUser,
                    AppPermission.UpdateUser,
                    AppPermission.DeleteUser,
                    AppPermission.CheckEmailBeUnique,
                ] 
            },
            { 
                AppAction.ConstituencyAdministration,
                [
                    AppPermission.AccessConstituenciesAdminPage,
                    AppPermission.GetConstituencies,
                    AppPermission.GetConstituency,
                    AppPermission.CreateConstituency,
                    AppPermission.UpdateConstituency,
                    AppPermission.DeletePollingStation,
                ] 
            },
            { 
                AppAction.PollingStationAdministration,
                [
                    AppPermission.AccessPollingStationsAdminPage,
                    AppPermission.GetPollingStation,
                    AppPermission.GetPollingStations,
                    AppPermission.CreatePollingStation,
                    AppPermission.UpdatePollingStation,
                    AppPermission.DeletePollingStation,
                ] 
            },
            {   // C'est l'action de recevoir les demandes et les valider / refuser / 
                AppAction.RegistrationRequestManagement,
                [
                    AppPermission.AccessRegistrationRequestsForManagementPage,
                    AppPermission.AccessUpdateRegistrationRequest,
                    AppPermission.GetRegistrationRequestsForManagement,
                    AppPermission.UpdateRegistrationRequestsForManagement,
                    AppPermission.DeleteRegistrationRequestsForManagement,
                    AppPermission.TriggerActionOnRegistrationRequest,
                    AppPermission.CheckRegistrationReferenceBeUnique,
                ] 
            },
            { 
                // C'est l'action de faire une nouvelle demande
                // et y apporter les modifications nécessaires 
                // si elle est incomplète
                AppAction.RegistrationRequestCreation,
                [
                    AppPermission.AccessUpdateRegistrationRequest,
                    AppPermission.CreateRegistrationRequest,
                    AppPermission.UpdateRegistrationRequest,
                    AppPermission.UploadRegistrationRequestTempDocument,
                    AppPermission.GetRegistrationRequestForCurrentUser,
                ] 
            },
            { 
                AppAction.RegistrationRequestConsultation,
                [
                    AppPermission.AccessRegistrationRequestsPage,
                    AppPermission.DeleteRegistrationRequest,
                    AppPermission.GetRegistrationRequests,
                    AppPermission.GetRegistrationRequestForCurrentUser,
                ]
            },
            {
                // C'est avoir tous les droits sur toutes les demandes
                AppAction.RegistrationRequestAdministration,
                [
                    AppPermission.AccessRegistrationRequestsForAdminPage,
                    AppPermission.AccessUpdateRegistrationRequest,
                    AppPermission.GetRegistrationRequest,
                ]
            },
        };

        public static readonly Dictionary<string, List<AppAction>> RolesSeed = new()
        {
            { AppConstants.SuperAdminRole, [AppAction.SuperAdmin] },
            {
                AppConstants.OrganismAgentRole,
                [
                    AppAction.RegistrationRequestConsultation,
                    AppAction.RegistrationRequestManagement,
                    AppAction.PollingStationAdministration,
                ]
            },
            {
                AppConstants.ElectorRole,
                [
                    AppAction.RegistrationRequestCreation,
                    AppAction.RegistrationRequestConsultation,
                ]
            }
        };
    }
}
