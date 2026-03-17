namespace Application.Common.Enums;

public enum AppPermission
{
    SuperAdmin,

    CreateUser,
    DeleteUser,
    GetCurrentUser,
    GetUser,
    GetUsers,

    CreateRole,
    UpdateRole,
    DeleteRole,
    GetRole,
    GetRoles,

    CreateConstituency,
    UpdateConstituency,
    DeleteConstituency,
    GetConstituency,
    GetConstituencies,

    CreatePollingStation,
    UpdatePollingStation,
    DeletePollingStation,
    GetPollingStation,
    GetPollingStations,

    CreateRegistrationRequest,
    UpdateRegistrationRequest,
    GetRegistrationRequest,
    GetRegistrationRequests,

    ImportExcelData,
    ExportExcelData,
}
