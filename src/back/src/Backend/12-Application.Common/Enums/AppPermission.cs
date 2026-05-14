namespace Application.Common.Enums;

public enum AppPermission
{
    SuperAdmin,

    AccessUsersAdminPage,
    CreateUser,
    UpdateUser,
    DeleteUser,
    GetCurrentUser,
    GetUser,
    GetUsers,
    CheckEmailBeUnique,
    GetRoles,
    GetProfile,

    AccessConstituenciesAdminPage,
    CreateConstituency,
    UpdateConstituency,
    DeleteConstituency,
    GetConstituency,
    GetConstituencies,

    AccessPollingStationsAdminPage,
    CreatePollingStation,
    UpdatePollingStation,
    DeletePollingStation,
    GetPollingStation,
    GetPollingStations,

    CreateRegistrationRequest,
    UpdateRegistrationRequest,
    DeleteRegistrationRequest,
    GetRegistrationRequest,
    GetRegistrationRequests,
    GetRegistrationRequestForCurrentUser,
    AccessUpdateRegistrationRequest,
    AccessRegistrationRequestsForAdminPage,
    AccessRegistrationRequestsForManagementPage,
    GetRegistrationRequestsForManagement,
    UpdateRegistrationRequestsForManagement,
    DeleteRegistrationRequestsForManagement,
    TriggerActionOnRegistrationRequest,
    CheckRegistrationReferenceBeUnique,
    AccessRegistrationRequestsPage,
    UploadRegistrationRequestTempDocument,
    UpdateRegistrationRequestDraft,

    ImportExcelData,
    ExportExcelData,
}
