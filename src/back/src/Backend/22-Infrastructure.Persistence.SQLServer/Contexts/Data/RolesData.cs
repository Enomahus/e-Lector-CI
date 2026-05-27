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
                    AppPermission.GetRoles,
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
                    AppPermission.DeleteConstituency,
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
            { // AGENT: Aaction de recevoir, consulter globalement et traiter (valider / refuser)
                AppAction.RegistrationRequestManagement,
                [
                    AppPermission.AccessRegistrationRequestsForManagementPage,
                    AppPermission.GetRegistrationRequestsForManagement,
                    AppPermission.UpdateRegistrationRequestsForManagement,
                    AppPermission.GetRegistrationRequests,
                    //AppPermission.DeleteRegistrationRequestsForManagement,
                    //AppPermission.AccessUpdateRegistrationRequest,
                    AppPermission.TriggerActionOnRegistrationRequest, // Approuver / Refuser
                    AppPermission.CheckRegistrationReferenceBeUnique,
                ]
            },
            {
                // ELECTEUR: Action de faire une nouvelle demande et la modifier
                // si incomplète et pas encore Approuver ou Refuser
                AppAction.RegistrationRequestCreation,
                [
                    AppPermission.CreateRegistrationRequest,
                    AppPermission.AccessUpdateRegistrationRequest,
                    AppPermission.UpdateRegistrationRequest,
                    AppPermission.UploadRegistrationRequestTempDocument,
                    //AppPermission.GetRegistrationRequestForCurrentUser,
                ]
            },
            {
                // ELECTEUR: Consultation restreinte à SES propre données
                AppAction.RegistrationRequestConsultation,
                [
                    AppPermission.AccessRegistrationRequestsPage,
                    //AppPermission.DeleteRegistrationRequest,
                    //AppPermission.GetRegistrationRequests,
                    AppPermission.GetRegistrationRequestForCurrentUser,
                ]
            },
            {
                // ADMIN: Tous les droits spécifiques sur toutes les demandes
                AppAction.RegistrationRequestAdministration,
                [
                    AppPermission.AccessRegistrationRequestsForAdminPage,
                    AppPermission.AccessUpdateRegistrationRequest,
                    AppPermission.GetRegistrationRequest,
                    AppPermission.DeleteRegistrationRequest,
                    AppPermission.GetRegistrationRequests,
                ]
            },
            {
                AppAction.CommonAccess,
                [
                    AppPermission.GetRoles,
                    AppPermission.GetProfile,
                    AppPermission.GetConstituencies,
                    AppPermission.GetPollingStations,
                ]
            },
        };

        public static readonly Dictionary<string, List<AppAction>> RolesSeed = new()
        {
            { AppConstants.SuperAdminRole, [AppAction.SuperAdmin] },
            {
                AppConstants.OrganismAgentRole,
                [
                    AppAction.CommonAccess,
                    //AppAction.RegistrationRequestConsultation,
                    AppAction.RegistrationRequestManagement,
                    AppAction.PollingStationAdministration,
                ]
            },
            {
                AppConstants.ElectorRole,
                [
                    AppAction.CommonAccess,
                    AppAction.RegistrationRequestCreation, // Creation et MAJ
                    AppAction.RegistrationRequestConsultation, // Lecture personnelle uniquement
                ]
            },
        };
    }
}
