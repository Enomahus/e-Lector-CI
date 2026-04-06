namespace Tools.Constants;

public static class AppConstants
{
    public static readonly string SuperAdminRole = "SuperAdmin";
    public static readonly string OrganismAgentRole = "OrganismAgentRole";
    public static readonly string ElectorRole = "ElectorRole";
    public static readonly string ImpersonatorIdClaim = "ImpersonatorId";
    public static readonly string ImpersonatorEmailClaim = "ImpersonatorEmail";

    // App links
    public const string ConfirmGuestRequestLink = "{0}/registration-request-confirm?token={1}&id={2}";
    public const string ConfirmPasswordResetLink = "{0}/reset-password?token={1}&email={2}";
}
